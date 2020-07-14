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
using DocumentFormat.OpenXml.Math;

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

            var alunosTurmas = await MontarAlunosTurmas(filtros);

            var turmas = new List<Turma>();

            foreach (var aluno in alunosTurmas)
            {
                foreach (var turma in aluno.Turmas)
                {
                    if (!turmas.Any(a => a.Codigo == turma.Codigo))
                        turmas.Add(turma);
                }
            }

            var alunosCodigo = alunosTurmas.Select(a => a.Aluno.Codigo);

            var turmasCodigo = turmas.Select(a => a.Codigo).Distinct();

            var componentesCurriculares = await ObterComponentesCurricularesTurmasRelatorio(turmasCodigo.ToArray(), filtros.UeCodigo, filtros.Modalidade, filtros.Usuario);

            var areasDoConhecimento = await ObterAreasConhecimento(componentesCurriculares);

            var dre = await ObterDrePorCodigo(filtros.DreCodigo);

            var ue = await ObterUePorCodigo(filtros.UeCodigo);

            var enderecoAtoUe = await ObterEnderecoAtoUe(filtros.UeCodigo);

            var tipoNotas = await ObterTiposNotaRelatorio();

            var notas = await ObterNotasAlunos(turmasCodigo.ToArray(), alunosCodigo.ToArray());
            var frequencias = await ObterFrequenciasAlunos(turmasCodigo.ToArray(), alunosCodigo.ToArray());

            var mediasFrequencia = await ObterMediasFrequencia();

            var resultadoFinal = await mediator.Send(new MontarHistoricoEscolarQuery(dre, ue, areasDoConhecimento, componentesCurriculares, alunosTurmas, mediasFrequencia, notas,
                frequencias, tipoNotas, turmasCodigo.ToArray(), cabecalho, legenda));

            var resultadoFinalFundamental = resultadoFinal.Where(a => a.Modalidade == Modalidade.Fundamental);
            var resultadoFinalMedio = resultadoFinal.Where(a => a.Modalidade == Modalidade.Medio);

            if (resultadoFinalFundamental.Any() || resultadoFinalMedio.Any())
            {
                if (resultadoFinalFundamental.Any() && resultadoFinalMedio.Any())
                {
                    await EnviaRelatorioFundamental(resultadoFinalFundamental, request.CodigoCorrelacao);

                    await EnviaRelatorioMedio(resultadoFinalMedio, request.CodigoCorrelacao);

                } else if (resultadoFinalFundamental.Any())
                {
                    await EnviaRelatorioFundamental(resultadoFinalFundamental, request.CodigoCorrelacao);
                } else if (resultadoFinalMedio.Any())
                {
                    await EnviaRelatorioMedio(resultadoFinalMedio, request.CodigoCorrelacao);
                }
            }
            else
                throw new NegocioException("Não foi possível localizar informações com os filtros selecionados");


        }

        private async Task EnviaRelatorioMedio(IEnumerable<HistoricoEscolarDTO> resultadoFinalMedio, Guid codigoCorrelacaoMedio)
        {
            var codigoCorrelacao = await mediator.Send(new GerarCodigoCorrelacaoSGPCommand(codigoCorrelacaoMedio));

            var jsonString = JsonConvert.SerializeObject(new { relatorioHistoricoEscolar = resultadoFinalMedio });
            await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioHistoricoEscolarFundamental/HistoricoEscolar", jsonString, TipoFormatoRelatorio.Pdf, codigoCorrelacao));
        }

        private async Task EnviaRelatorioFundamental(IEnumerable<HistoricoEscolarDTO> resultadoFinalFundamental, Guid codigoCorrelacao)
        {
            var jsonString = JsonConvert.SerializeObject(new { relatorioHistoricoEscolar = resultadoFinalFundamental });
            await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioHistoricoEscolarMedio/HistoricoEscolar", jsonString, TipoFormatoRelatorio.Pdf, codigoCorrelacao));
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
            return await mediator.Send(new ObterUePorCodigoQuery()
            {
                UeCodigo = ueCodigo
            });
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

        private async Task<CabecalhoDto> MontarCabecalho(FiltroHistoricoEscolarDto filtros)
        {
            var enderecosEAtos = await mediator.Send(new ObterEnderecoEAtosDaUeQuery(filtros.UeCodigo));
            if (!enderecosEAtos.Any())
                throw new NegocioException("Não foi possível obter os dados de endereço e atos da UE.");

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

        private async Task<IEnumerable<MediaFrequencia>> ObterMediasFrequencia()
        {
            return await mediator.Send(new ObterParametrosMediaFrequenciaQuery());
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
            return resultado.Substring(0, resultado.Length-2);
        }
    }
}
