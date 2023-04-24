using MediatR;
using Newtonsoft.Json;
using SME.SR.Data;
using SME.SR.Data.Models;
using SME.SR.Infra;
using SME.SR.Infra.RelatorioPaginado;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            request.RotaErro = RotasRabbitSGP.RotaRelatoriosComErroHistoricoEscolar;
            var filtros = request.ObterObjetoFiltro<FiltroHistoricoEscolarDto>();

            var legenda = await ObterLegenda();

            var cabecalho = await MontarCabecalho(filtros);

            var alunosTurmas = await MontarAlunosTurmas(filtros);

            var alunosTurmasTransferencia = await MontarAlunosTurmasTransferencia(filtros);

            if ((alunosTurmas == null || !alunosTurmas.Any()) &&
                (alunosTurmasTransferencia == null || !alunosTurmasTransferencia.Any()))
                throw new NegocioException("Não foi encontrado nenhum histórico de promoção e transferência para o(s) aluno(s) da turma.");

            var alunosTransferenciaSemHistorico = alunosTurmasTransferencia.Where(tf => !alunosTurmas.Select(a => a.Aluno.Codigo).Contains(tf.Aluno.Codigo));

            var todosAlunosTurmas = new List<AlunoTurmasHistoricoEscolarDto>();

            if (alunosTransferenciaSemHistorico != null && alunosTransferenciaSemHistorico.Any())
            {
                filtros.AlunosCodigo = alunosTransferenciaSemHistorico.Select(a => a.Aluno.Codigo).ToArray();
                var alunosTransferenciaTurmas = await MontarAlunosTurmas(filtros);

                if (alunosTransferenciaTurmas != null && alunosTransferenciaTurmas.Any())
                    todosAlunosTurmas.AddRange(alunosTransferenciaTurmas);
            }

            if (alunosTurmas != null && alunosTurmas.Any())
                todosAlunosTurmas.AddRange(alunosTurmas);

            if (alunosTurmasTransferencia != null && alunosTurmasTransferencia.Any())
                todosAlunosTurmas.AddRange(alunosTurmasTransferencia);

            var todasTurmas = todosAlunosTurmas.SelectMany(a => a.Turmas).DistinctBy(t => t.Codigo);
            var todosAlunos = todosAlunosTurmas.Select(a => a.Aluno).DistinctBy(t => t.Codigo);

            var todasTurmasAssociadas = todosAlunosTurmas.SelectMany(a => a.Turmas)
                                            .Where(t => !string.IsNullOrEmpty(t.RegularCodigo))?
                                            .DistinctBy(t => t.Codigo)?
                                            .Select(t => (t.Codigo, t.RegularCodigo));

            var turmasAssociadasCodigo = todasTurmasAssociadas?.Select(a => a.Codigo);

            var turmasCodigo = todasTurmas.Select(a => a.Codigo);
            var alunosCodigo = todosAlunos.Select(a => a.Codigo);

            IEnumerable<IGrouping<(long, Modalidade), UeConclusaoPorAlunoAno>> historicoUes = null;

            if (todosAlunos != null && todosAlunos.Any())
            {
                historicoUes = !filtros.AlunosCodigo?.Any() ?? true
                    ? await ObterUesConclusaoParaTurma(alunosCodigo, filtros.Modalidade)
                    : await ObterUesConclusaoParaAluno(alunosCodigo, todosAlunosTurmas);
            }

            var notas = await ObterNotasAlunos(alunosCodigo.ToArray(), filtros.AnoLetivo, filtros.Modalidade, filtros.Semestre);
            var frequencias = await ObterFrequenciasAlunos(alunosCodigo.ToArray(), filtros.AnoLetivo, filtros.Modalidade, filtros.Semestre);

            var componentesCurriculares = await ObterComponentesCurricularesTurmasRelatorio(turmasCodigo.ToArray(), filtros.UeCodigo, filtros.Modalidade, filtros.Usuario);

            if (componentesCurriculares.Any(cc => turmasAssociadasCodigo.Contains(cc.Key)))
                componentesCurriculares = ConverterTurmasAssociadasParaRegular(componentesCurriculares, turmasAssociadasCodigo, todasTurmasAssociadas);

            var registroFrequenciasAlunos = await mediator.Send(new ObterRegistrosFrequenciasAlunoQuery(alunosCodigo.ToArray(), turmasCodigo.ToArray(), new string[] { }, 0, new int[] { }));

            if (notas.Any(n => turmasAssociadasCodigo.Contains(n.Key)))
                notas = ConverterTurmasAssociadasParaRegular(notas, turmasAssociadasCodigo, todasTurmasAssociadas);

            if (frequencias.Any(n => turmasAssociadasCodigo.Contains(n.Key)))
                frequencias = ConverterTurmasAssociadasParaRegular(frequencias, turmasAssociadasCodigo, todasTurmasAssociadas);

            var areasDoConhecimento = await ObterAreasConhecimento(componentesCurriculares);

            var ordenacaoGrupoArea = await ObterOrdenacaoAreasConhecimento(componentesCurriculares, areasDoConhecimento);

            var dre = await ObterDrePorCodigo(filtros.DreCodigo);

            var ue = await ObterUePorCodigo(filtros.UeCodigo);

            var bimestreAtual = await mediator.Send(new ObterBimestrePeriodoFechamentoAtualQuery(filtros.AnoLetivo));

            var enderecoAtoUe = await ObterEnderecoAtoUe(filtros.UeCodigo);

            var tipoNotas = await ObterTiposNotaRelatorio();

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
            IEnumerable<HistoricoEscolarEJADto> resultadoEJA = null, resultadoFinalEJA = null;
            IEnumerable<TransferenciaDto> resultadoTransferencia = null;

            if (turmasTransferencia != null && turmasTransferencia.Any())
                resultadoTransferencia = await mediator.Send(new MontarHistoricoEscolarTransferenciaQuery(areasDoConhecimento, ordenacaoGrupoArea, componentesCurriculares, alunosTurmasTransferencia, mediasFrequencia, notas,
                  frequencias, tipoNotas, turmasTransferencia.Select(a => a.Codigo).Distinct().ToArray(), legenda, registroFrequenciasAlunos, bimestreAtual));

            if ((turmasFundMedio != null && turmasFundMedio.Any() && (filtros.Modalidade == Modalidade.Fundamental || filtros.Modalidade == Modalidade.Medio) ) 
                || (turmasTransferencia != null && turmasTransferencia.Any(t => t.ModalidadeCodigo != Modalidade.EJA)))
                resultadoFundMedio = await mediator.Send(new MontarHistoricoEscolarQuery(dre, ue, areasDoConhecimento, componentesCurriculares, ordenacaoGrupoArea, todosAlunosTurmas, mediasFrequencia, notas,
                    frequencias, tipoNotas, resultadoTransferencia, turmasFundMedio?.Select(a => a.Codigo).Distinct().ToArray(), cabecalho, legenda, dadosData, dadosDiretor, dadosSecretario,
                    historicoUes, filtros.PreencherDataImpressao, filtros.ImprimirDadosResponsaveis, filtros.InformarObservacoesComplementares ? filtros.ObservacaoComplementar : null));

            if ((turmasEja != null && turmasEja.Any() && filtros.Modalidade == Modalidade.EJA) 
                || (turmasTransferencia != null && turmasTransferencia.Any(t => t.ModalidadeCodigo == Modalidade.EJA)))
                    resultadoEJA = await mediator.Send(new MontarHistoricoEscolarEJAQuery(dre, ue, areasDoConhecimento, componentesCurriculares, ordenacaoGrupoArea, todosAlunosTurmas, mediasFrequencia, notas,
                        frequencias, tipoNotas, resultadoTransferencia, turmasEja?.Select(a => a.Codigo).Distinct().ToArray(), cabecalho, legenda, dadosData, dadosDiretor, dadosSecretario,
                        historicoUes, filtros.PreencherDataImpressao, filtros.ImprimirDadosResponsaveis));

            if (resultadoFundMedio != null && resultadoFundMedio.Any())
            {
                resultadoFinalFundamental = resultadoFundMedio.Where(a => a.Modalidade == Modalidade.Fundamental);
                resultadoFinalMedio = resultadoFundMedio.Where(a => a.Modalidade == Modalidade.Medio);
            }
            else if (resultadoEJA != null && resultadoEJA.Any())
                resultadoFinalEJA = resultadoEJA.Where(a => a.Modalidade == Modalidade.EJA);

            if ((resultadoFinalFundamental != null && resultadoFinalFundamental.Any()) ||
                (resultadoFinalMedio != null && resultadoFinalMedio.Any()) ||
                (resultadoEJA != null && resultadoEJA.Any()))
            {

                if (resultadoFinalMedio != null && resultadoFinalMedio.Any() && filtros.Modalidade == Modalidade.Medio)
                {
                    await EnviaRelatorioMedio(resultadoFinalMedio, request.CodigoCorrelacao);
                    request.CodigoCorrelacao = await CopiarCorrelacao(request.CodigoCorrelacao);
                }

                if (resultadoEJA != null && resultadoEJA.Any() && filtros.Modalidade == Modalidade.EJA)
                    await EnviaRelatorioEJA(resultadoFinalEJA, request.CodigoCorrelacao);

                if (resultadoFinalFundamental != null && resultadoFinalFundamental.Any() && filtros.Modalidade == Modalidade.Fundamental)
                    await EnviaRelatorioFundamental(resultadoFinalFundamental, request.CodigoCorrelacao);

                
            }
            else
                throw new NegocioException("Não foi possível localizar informações com os filtros selecionados");
        }

        private IEnumerable<IGrouping<string, FrequenciaAluno>> ConverterTurmasAssociadasParaRegular(IEnumerable<IGrouping<string, FrequenciaAluno>> frequencias, IEnumerable<string> turmasAssociadasCodigo, IEnumerable<(string Codigo, string RegularCodigo)> todasTurmasAssociadas)
        {
            var frequenciasLista = frequencias.SelectMany(e => e).ToList();

            foreach (var frequencia in frequenciasLista.Where(cc => turmasAssociadasCodigo.Contains(cc.TurmaId)))
            {
                var turmaRegularId = todasTurmasAssociadas.FirstOrDefault(tr => tr.Codigo == frequencia.TurmaId).RegularCodigo;

                frequencia.TurmaId = turmaRegularId;
            }

            return frequenciasLista.GroupBy(cc => cc.TurmaId);
        }

        private IEnumerable<IGrouping<string, NotasAlunoBimestre>> ConverterTurmasAssociadasParaRegular(IEnumerable<IGrouping<string, NotasAlunoBimestre>> notas, IEnumerable<string> turmasAssociadasCodigo, IEnumerable<(string Codigo, string RegularCodigo)> todasTurmasAssociadas)
        {
            var notasLista = notas.SelectMany(e => e).ToList();

            foreach (var nota in notasLista.Where(cc => turmasAssociadasCodigo.Contains(cc.CodigoTurma)))
            {
                var turmaRegularId = todasTurmasAssociadas.FirstOrDefault(tr => tr.Codigo == nota.CodigoTurma).RegularCodigo;

                nota.CodigoTurma = turmaRegularId;
            }

            return notasLista.GroupBy(cc => cc.CodigoTurma);
        }

        private IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> ConverterTurmasAssociadasParaRegular(IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> componentesCurriculares, IEnumerable<string> turmasAssociadasCodigo, IEnumerable<(string Codigo, string RegularCodigo)> todasTurmasAssociadas)
        {
            var componentesLista = componentesCurriculares.SelectMany(e => e).ToList();

            foreach (var componenteDaTurma in componentesLista.Where(cc => turmasAssociadasCodigo.Contains(cc.CodigoTurma)))
            {
                var turmaRegularId = todasTurmasAssociadas.FirstOrDefault(tr => tr.Codigo == componenteDaTurma.CodigoTurma).RegularCodigo;

                componenteDaTurma.CodigoTurmaAssociada = componenteDaTurma.CodigoTurma;
                componenteDaTurma.CodigoTurma = turmaRegularId;
            }

            return componentesLista.GroupBy(cc => cc.CodigoTurma);
        }

        private async Task<IEnumerable<IGrouping<string, NotasAlunoBimestre>>> ObterNotasAlunos(string[] alunosCodigo, int anoLetivo, Modalidade modalidade, int semestre)
        {
            return await mediator.Send(new ObterNotasRelatorioHistoricoEscolarQuery(alunosCodigo, anoLetivo, (int)modalidade, semestre));
        }

        private async Task<IEnumerable<IGrouping<string, FrequenciaAluno>>> ObterFrequenciasAlunos(string[] alunosCodigo, int anoLetivo, Modalidade modalidade, int semestre)
        {
            return await mediator.Send(new ObterFrequenciasRelatorioHistoricoEscolarQuery(alunosCodigo, anoLetivo, modalidade, semestre));
        }

        private async Task EnviaRelatorioMedio(IEnumerable<HistoricoEscolarDTO> resultadoFinalMedio, Guid codigoCorrelacaoMedio)
        {
            var jsonString = JsonConvert.SerializeObject(new { relatorioHistoricoEscolar = resultadoFinalMedio });
            await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioHistoricoEscolarMedio/HistoricoEscolar", jsonString, TipoFormatoRelatorio.Pdf, codigoCorrelacaoMedio, RotasRabbitSR.RotaRelatoriosProcessandoHistoricoEscolar));
        }

        private async Task<Guid> CopiarCorrelacao(Guid codigoCorrelacaoMedio)
        {
            return await mediator.Send(new GerarCodigoCorrelacaoSGPCommand(codigoCorrelacaoMedio));
        }

        private async Task EnviaRelatorioFundamental(IEnumerable<HistoricoEscolarDTO> resultadoFinalFundamental, Guid codigoCorrelacao)
        {
            var relatorioPaginados = new RelatorioPaginadoHistoricoEscolar(resultadoFinalFundamental);

            await mediator.Send(new GerarRelatorioHtmlPDFHistoricoEscolarCommand(relatorioPaginados.ObterRelatorioPaginadoFundamental(), codigoCorrelacao));
        }

        private async Task EnviaRelatorioEJA(IEnumerable<HistoricoEscolarEJADto> resultadoFinalEJA, Guid codigoCorrelacao)
        {
            var jsonString = JsonConvert.SerializeObject(new { relatorioHistoricoEscolar = resultadoFinalEJA });
            await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioHistoricoEscolarEja/HistoricoEscolar", jsonString, TipoFormatoRelatorio.Pdf, codigoCorrelacao, RotasRabbitSR.RotaRelatoriosProcessandoHistoricoEscolar));
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

        private async Task<IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto>> ObterOrdenacaoAreasConhecimento(IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> componentesCurriculares, IEnumerable<AreaDoConhecimento> areasDoConhecimento)
        {
            var listaGrupoMatrizId = componentesCurriculares?.SelectMany(a => a)?.Select(a => a.GrupoMatriz.Id)?.Distinct().ToArray();
            var listaAreaConhecimentoId = areasDoConhecimento?.Select(a => a.Id).ToArray();

            return await mediator.Send(new ObterComponenteCurricularGrupoAreaOrdenacaoQuery(listaGrupoMatrizId, listaAreaConhecimentoId));
        }

        private async Task<IEnumerable<AlunoTurmasHistoricoEscolarDto>> MontarAlunosTurmas(FiltroHistoricoEscolarDto filtros)
        {

            long[] alunosCodigos;
            if (filtros.AlunosCodigo != null && filtros.AlunosCodigo.Length > 0 && !filtros.AlunosCodigo.Any(a => a is null))
                alunosCodigos = filtros.AlunosCodigo.Select(long.Parse).ToArray();
            else
                alunosCodigos = new long[0];

            var turmaCodigo = string.IsNullOrEmpty(filtros.TurmaCodigo) ? 0 : long.Parse(filtros.TurmaCodigo);

            return await mediator.Send(new ObterAlunosETurmasHistoricoEscolarQuery(turmaCodigo, alunosCodigos));
        }

        private async Task<IEnumerable<AlunoTurmasHistoricoEscolarDto>> MontarAlunosTurmasTransferencia(FiltroHistoricoEscolarDto filtros)
        {

            long[] alunosCodigos;
            if (filtros.AlunosCodigo != null && filtros.AlunosCodigo.Length > 0 && !filtros.AlunosCodigo.Any(a => a is null))
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
                var cabecalho = new CabecalhoDto
                {
                    NomeUe = enderecosEAtos?.FirstOrDefault()?.NomeUe?.ToUpper(),
                    Endereco = enderecosEAtos?.FirstOrDefault()?.Endereco?.ToUpper(),
                    AtoCriacao = enderecosEAtos?.FirstOrDefault(teste => teste.TipoOcorrencia == "1")?.Atos?.ToUpper(),
                    AtoAutorizacao = enderecosEAtos?.FirstOrDefault(teste => teste.TipoOcorrencia == "7")?.Atos?.ToUpper()
                };
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

        private async Task<IEnumerable<IGrouping<(long, Modalidade), UeConclusaoPorAlunoAno>>> ObterUesConclusaoParaTurma(IEnumerable<string> alunosCodigo, Modalidade modalidade)
        {
            var alunosCodigosFiltro = alunosCodigo.Select(long.Parse).ToArray();

            return await mediator.Send(new ObterUesConclusaoQuery()
            {
                CodigosAlunos = alunosCodigosFiltro,
                Modalidade = modalidade
            });
        }

        private async Task<IEnumerable<IGrouping<(long, Modalidade), UeConclusaoPorAlunoAno>>> ObterUesConclusaoParaAluno(IEnumerable<string> alunosCodigo, List<AlunoTurmasHistoricoEscolarDto> alunosTurmas)
        {
            var alunosCodigosFiltro = alunosCodigo
                .Select(long.Parse)
                .ToArray();

            var modalidades = alunosTurmas
                .SelectMany(x => x.Turmas)
                .Where(x => x.TipoTurma == TipoTurma.Regular)
                .Select(x => x.ModalidadeCodigo)
                .Distinct();

            var retorno = new List<IGrouping<(long, Modalidade), UeConclusaoPorAlunoAno>>();

            foreach (var modalidade in modalidades)
            {
                retorno.AddRange(await mediator.Send(new ObterUesConclusaoQuery()
                {
                    CodigosAlunos = alunosCodigosFiltro,
                    Modalidade = modalidade
                }));
            }

            return retorno;
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

        private async Task<LegendaDto> ObterLegenda()
        {
            var legenda = new LegendaDto
            {
                TextoConceito = await ObterLegendaConceito(),
                TextoSintese = await ObterLegendaSintese()
            };
            legenda.Texto = legenda.TextoConceito;
            return legenda;
        }

        private async Task<string> ObterLegendaConceito()
        {
            var sb = new StringBuilder();
            var legendas = await mediator.Send(new ObterLegendaQuery(TipoLegenda.Conceito));
            foreach (var conceito in legendas)
            {
                sb.Append($"{conceito.Valor} = {conceito.Descricao}, ");
            }

            var resultado = sb.ToString().Replace("\t", "");
            return resultado.Substring(0, resultado.Length - 2);
        }

        private async Task<string> ObterLegendaSintese()
        {
            var sb = new StringBuilder();
            var legendas = await mediator.Send(new ObterLegendaQuery(TipoLegenda.Sintese));
            foreach (var conceito in legendas)
            {
                sb.Append($"{conceito.Valor} = {conceito.Descricao}, ");
            }

            var resultado = sb.ToString().Replace("\t", "");
            return resultado.Substring(0, resultado.Length - 2);
        }
    }
}
