using MediatR;
using Newtonsoft.Json;
using SME.SR.Data;
using SME.SR.Data.Models;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SME.SR.Infra.Utilitarios;

namespace SME.SR.Application
{
    public class RelatorioHistoricoEscolarUseCase : IRelatorioHistoricoEscolarUseCase
    {
        private readonly IMediator mediator;

        public RelatorioHistoricoEscolarUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<FiltroHistoricoEscolarDto>();

            var legenda = new LegendaDto(await ObterLegenda());

            var cabecalho = await MontarCabecalho(filtros);

            var alunosTurmasTransferencia = await MontarAlunosTurmasTransferencia(filtros);

            var alunosTurmas = await MontarAlunosTurmas(filtros);

            if ((alunosTurmas == null || !alunosTurmas.Any()) &&
                (alunosTurmasTransferencia == null || !alunosTurmasTransferencia.Any()))
                throw new NegocioException("Não foi encontrado nenhum histórico de promoção e transferência para o(s) aluno(s) da turma.");

            var todosAlunosTurmas = new List<AlunoTurmasHistoricoEscolarDto>();

            if (alunosTurmas != null && alunosTurmas.Any())
                todosAlunosTurmas.AddRange(alunosTurmas);

            if (alunosTurmasTransferencia != null && alunosTurmasTransferencia.Any())
                todosAlunosTurmas.AddRange(alunosTurmasTransferencia);

            var todasTurmas = todosAlunosTurmas.SelectMany(a => a.Turmas).DistinctBy(t => t.Codigo);
            var todosAlunos = todosAlunosTurmas.Select(a => a.Aluno).DistinctBy(t => t.Codigo);

            var turmasCodigo = todasTurmas.Select(a => a.Codigo);
            var alunosCodigo = todosAlunos.Select(a => a.Codigo);

            var componentesCurriculares = await ObterComponentesCurricularesTurmasRelatorio(turmasCodigo.ToArray(), filtros.UeCodigo, filtros.Modalidade, filtros.Usuario);

            var areasDoConhecimento = await ObterAreasConhecimento(componentesCurriculares);

            var dre = await ObterDrePorCodigo(filtros.DreCodigo);

            var ue = await ObterUePorCodigo(filtros.UeCodigo);

            var enderecoAtoUe = await ObterEnderecoAtoUe(filtros.UeCodigo);

            var tipoNotas = await ObterTiposNotaRelatorio();

            var notas = await ObterNotasAlunos(turmasCodigo.ToArray(), alunosCodigo.ToArray());
            var frequencias = await ObterFrequenciasAlunos(turmasCodigo.ToArray(), alunosCodigo.ToArray());

            var mediasFrequencia = await ObterMediasFrequencia();

            var dadosData = await ObterDadosData(filtros.PreencherDataImpressao);

            FuncionarioDto dadosDiretor = null, dadosSecretario = null;

            if (filtros.ImprimirDadosResponsaveis)
            {
                dadosDiretor = await ObterFuncionarioUePorCargo(filtros.UeCodigo, (int)Cargo.Diretor);
                dadosSecretario = await ObterFuncionarioUePorCargo(filtros.UeCodigo, (int)Cargo.Secretario);
            }

            var turmasHistorico = alunosTurmas.SelectMany(t => t.Turmas).DistinctBy(t => t.Codigo);
            var turmasTransferencia = alunosTurmasTransferencia.SelectMany(t => t.Turmas).DistinctBy(t => t.Codigo);

            var turmasEja = turmasHistorico.Where(t => t.ModalidadeCodigo == Modalidade.EJA);
            var turmasFundMedio = turmasHistorico.Where(t => t.ModalidadeCodigo != Modalidade.EJA);

            IEnumerable<HistoricoEscolarDTO> resultadoFundMedio = null, resultadoFinalFundamental = null, resultadoFinalMedio = null;
            IEnumerable<HistoricoEscolarEJADto> resultadoEJA = null;
            IEnumerable<TransferenciaDto> resultadoTransferencia = null;

            if (turmasTransferencia != null && turmasTransferencia.Any())
                resultadoTransferencia = await mediator.Send(new MontarHistoricoEscolarTransferenciaQuery(areasDoConhecimento, componentesCurriculares, alunosTurmasTransferencia, mediasFrequencia, notas,
                  frequencias, turmasTransferencia.Select(a => a.Codigo).Distinct().ToArray()));

            if (filtros.Modalidade != Modalidade.EJA)
                resultadoFundMedio = await mediator.Send(new MontarHistoricoEscolarQuery(dre, ue, areasDoConhecimento, componentesCurriculares, todosAlunosTurmas, mediasFrequencia, notas,
                    frequencias, tipoNotas, resultadoTransferencia, turmasFundMedio?.Select(a => a.Codigo).Distinct().ToArray(), cabecalho, legenda, dadosData, dadosDiretor, dadosSecretario,
                    filtros.PreencherDataImpressao, filtros.ImprimirDadosResponsaveis));
            else
                resultadoEJA = await mediator.Send(new MontarHistoricoEscolarEJAQuery(dre, ue, areasDoConhecimento, componentesCurriculares, todosAlunosTurmas, mediasFrequencia, notas,
                    frequencias, tipoNotas, resultadoTransferencia, turmasEja?.Select(a => a.Codigo).Distinct().ToArray(), cabecalho, legenda, dadosData, dadosDiretor, dadosSecretario,
                    filtros.PreencherDataImpressao, filtros.ImprimirDadosResponsaveis));

            if (resultadoFundMedio != null && resultadoFundMedio.Any())
            {
                resultadoFinalFundamental = resultadoFundMedio.Where(a => a.Modalidade == Modalidade.Fundamental);
                resultadoFinalMedio = resultadoFundMedio.Where(a => a.Modalidade == Modalidade.Medio);

                foreach (var item in resultadoFinalMedio)
                {
                    item.Legenda.Texto = string.Empty;
                }
            }

            if ((resultadoFinalFundamental != null && resultadoFinalFundamental.Any()) ||
                (resultadoFinalMedio != null && resultadoFinalMedio.Any()) ||
                (resultadoEJA != null && resultadoEJA.Any()))
            {
                if (resultadoEJA != null && resultadoEJA.Any())
                {
                    await EnviaRelatorioEJA(resultadoEJA, request.CodigoCorrelacao);
                }

                if (resultadoFinalFundamental != null && resultadoFinalFundamental.Any())
                {
                    await EnviaRelatorioFundamental(resultadoFinalFundamental, request.CodigoCorrelacao);
                }

                if (resultadoFinalMedio != null && resultadoFinalMedio.Any())
                {
                    await EnviaRelatorioMedio(resultadoFinalMedio, request.CodigoCorrelacao);
                }
            }
            else
                throw new NegocioException("Não foi possível localizar informações com os filtros selecionados");
        }

        private async Task<IEnumerable<IGrouping<string, NotasAlunoBimestre>>> ObterNotasAlunos(string[] turmasCodigo, string[] alunosCodigo)
        {
            return await mediator.Send(new ObterNotasRelatorioBoletimQuery()
            {
                CodigosAlunos = alunosCodigo,
                CodigosTurma = turmasCodigo
            });
        }

        private async Task<IEnumerable<IGrouping<string, FrequenciaAluno>>> ObterFrequenciasAlunos(string[] turmasCodigo, string[] alunosCodigo)
        {
            return await mediator.Send(new ObterFrequenciasRelatorioBoletimQuery()
            {
                CodigosAluno = alunosCodigo,
                CodigosTurma = turmasCodigo
            });
        }

        private async Task EnviaRelatorioMedio(IEnumerable<HistoricoEscolarDTO> resultadoFinalMedio, Guid codigoCorrelacaoMedio)
        {
            var codigoCorrelacao = await mediator.Send(new GerarCodigoCorrelacaoSGPCommand(codigoCorrelacaoMedio));

            var jsonString = JsonConvert.SerializeObject(new { relatorioHistoricoEscolar = resultadoFinalMedio });
            await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioHistoricoEscolarMedio/HistoricoEscolar", jsonString, TipoFormatoRelatorio.Pdf, codigoCorrelacao));
        }

        private async Task EnviaRelatorioFundamental(IEnumerable<HistoricoEscolarDTO> resultadoFinalFundamental, Guid codigoCorrelacao)
        {
            var jsonString = JsonConvert.SerializeObject(new { relatorioHistoricoEscolar = resultadoFinalFundamental });
            await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioHistoricoEscolarFundamental/HistoricoEscolar", jsonString, TipoFormatoRelatorio.Pdf, codigoCorrelacao));
        }

        private async Task EnviaRelatorioEJA(IEnumerable<HistoricoEscolarEJADto> resultadoFinalEJA, Guid codigoCorrelacao)
        {
            var jsonString = JsonConvert.SerializeObject(new { relatorioHistoricoEscolar = resultadoFinalEJA });
            await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioHistoricoEscolarEja/HistoricoEscolar", jsonString, TipoFormatoRelatorio.Pdf, codigoCorrelacao));
        }

        private async Task<IEnumerable<EnderecoEAtosDaUeDto>> ObterEnderecoAtoUe(string ueCodigo)
        {
            return await mediator.Send(new ObterEnderecoEAtosDaUeQuery(ueCodigo));
        }

        private async Task<Dre> ObterDrePorCodigo(string dreCodigo)
        {
            return await mediator.Send(new ObterDrePorCodigoQuery()
            {
                DreCodigo = dreCodigo
            });
        }

        private async Task<Ue> ObterUePorCodigo(string ueCodigo)
        {
            return await mediator.Send(new ObterUePorCodigoQuery(ueCodigo));
        }
        private async Task<IEnumerable<TipoNotaCicloAno>> ObterTiposNotaRelatorio()
        {
            return await mediator.Send(new ObterTiposNotaRelatorioHistoricoEscolarQuery());
        }
        private async Task<IEnumerable<AreaDoConhecimento>> ObterAreasConhecimento(IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> componentesCurriculares)
        {
            //TODO: MELHORAR CODIGO
            var listaCodigosComponentes = new List<long>();

            listaCodigosComponentes.AddRange(componentesCurriculares.SelectMany(a => a).Where(cc => cc.Regencia).SelectMany(a => a.ComponentesCurricularesRegencia).Select(cc => cc.CodDisciplina));
            listaCodigosComponentes.AddRange(componentesCurriculares.SelectMany(a => a).Where(cc => !cc.Regencia).Select(a => a.CodDisciplina));

            return await mediator.Send(new ObterAreasConhecimentoComponenteCurricularQuery(listaCodigosComponentes.Distinct().ToArray()));
        }

        private async Task<IEnumerable<AlunoTurmasHistoricoEscolarDto>> MontarAlunosTurmas(FiltroHistoricoEscolarDto filtros)
        {

            long[] alunosCodigos;
            if (!filtros.AlunosCodigo.Any(a => a is null))
                alunosCodigos = filtros.AlunosCodigo.Select(long.Parse).ToArray();
            else alunosCodigos = new long[0];

            var turmaCodigo = string.IsNullOrEmpty(filtros.TurmaCodigo) ? 0 : long.Parse(filtros.TurmaCodigo);

            return await mediator.Send(new ObterAlunosETurmasHistoricoEscolarQuery(turmaCodigo, alunosCodigos));
        }

        private async Task<IEnumerable<AlunoTurmasHistoricoEscolarDto>> MontarAlunosTurmasTransferencia(FiltroHistoricoEscolarDto filtros)
        {

            long[] alunosCodigos;
            if (!filtros.AlunosCodigo.Any(a => a is null))
                alunosCodigos = filtros.AlunosCodigo.Select(long.Parse).ToArray();
            else alunosCodigos = new long[0];

            var turmaCodigo = string.IsNullOrEmpty(filtros.TurmaCodigo) ? 0 : long.Parse(filtros.TurmaCodigo);

            return await mediator.Send(new ObterAlunosETurmasHistoricoEscolarTransferenciaQuery(turmaCodigo, alunosCodigos));
        }

        private async Task<CabecalhoDto> MontarCabecalho(FiltroHistoricoEscolarDto filtros)
        {
            var enderecosEAtos = await mediator.Send(new ObterEnderecoEAtosDaUeQuery(filtros.UeCodigo));

            if (enderecosEAtos == null || !enderecosEAtos.Any())
                enderecosEAtos = new List<EnderecoEAtosDaUeDto>();

            return MontaCabecalhoComBaseNoEnderecoEAtosDaUe(enderecosEAtos);
        }

        private static CabecalhoDto MontaCabecalhoComBaseNoEnderecoEAtosDaUe(IEnumerable<EnderecoEAtosDaUeDto> enderecosEAtos)
        {
            if (enderecosEAtos.Any())
            {
                var cabecalho = new CabecalhoDto();
                cabecalho.NomeUe = enderecosEAtos?.FirstOrDefault()?.NomeUe;
                cabecalho.Endereco = enderecosEAtos?.FirstOrDefault()?.Endereco;
                cabecalho.AtoCriacao = enderecosEAtos?.FirstOrDefault(teste => teste.TipoOcorrencia == "1")?.Atos;
                cabecalho.AtoAutorizacao = enderecosEAtos?.FirstOrDefault(teste => teste.TipoOcorrencia == "7")?.Atos;
                return cabecalho;
            }
            else return default;
        }

        private async Task<IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>>> ObterComponentesCurricularesTurmasRelatorio(string[] turmaCodigo, string codigoUe, Modalidade modalidade, Usuario usuario)
        {
            return await mediator.Send(new ObterComponentesCurricularesTurmasRelatorioBoletimQuery()
            {
                CodigosTurma = turmaCodigo,
                CodigoUe = codigoUe,
                Modalidade = modalidade,
                Usuario = usuario
            });
        }

        private async Task<IEnumerable<MediaFrequencia>> ObterMediasFrequencia()
        {
            return await mediator.Send(new ObterParametrosMediaFrequenciaQuery());
        }

        private async Task<DadosDataDto> ObterDadosData(bool preencherDarta)
        {
            return await mediator.Send(new ObterDadosDataQuery()
            {
                PreencherData = preencherDarta
            });
        }

        private async Task<FuncionarioDto> ObterFuncionarioUePorCargo(string codigoUe, int cargo)
        {
            var funcionarios = await mediator.Send(new ObterFuncionarioUePorCargoQuery()
            {
                CodigoCargo = cargo.ToString(),
                CodigoUe = codigoUe
            });

            if (funcionarios != null && funcionarios.Count() == 1)
                return funcionarios.FirstOrDefault();

            else return null;
        }

        private async Task<string> ObterLegenda()
        {
            var sb = new StringBuilder();
            var legendas = await mediator.Send(new ObterLegendaQuery());
            foreach (var conceito in legendas)
            {
                sb.Append($"{conceito.Valor} = {conceito.Descricao}, ");
            }

            var resultado = sb.ToString().Replace("\t", "");
            return resultado.Substring(0, resultado.Length - 2);
        }
    }
}
