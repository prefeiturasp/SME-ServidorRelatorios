using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using SME.SR.Application.Queries;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Extensions;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioSondagemPortuguesCapLeituraPorTurmaQueryHandler : IRequestHandler<ObterRelatorioSondagemPortuguesCapLeituraPorTurmaQuery, RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaDto>
    {
        private readonly IRelatorioSondagemPortuguesPorTurmaRepository relatorioSondagemPortuguesPorTurmaRepository;
        private readonly IPerguntasAutoralRepository perguntasAutoralRepository;
        private readonly IAlunoRepository alunoRepository;
        private readonly IMediator mediator;
        private readonly char[] lstChaves = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private const int SEGUNDO_BIMESTRE = 2;
        private const int PRIMEIRO_SEMESTRE = 1;
        private const int SEGUNDO_SEMESTRE = 2;

        public ObterRelatorioSondagemPortuguesCapLeituraPorTurmaQueryHandler(
            IRelatorioSondagemPortuguesPorTurmaRepository relatorioSondagemPortuguesPorTurmaRepository,
            IMediator mediator, IPerguntasAutoralRepository perguntasAutoralRepository, IAlunoRepository alunoRepository)
        {
            this.relatorioSondagemPortuguesPorTurmaRepository = relatorioSondagemPortuguesPorTurmaRepository ?? throw new ArgumentNullException(nameof(relatorioSondagemPortuguesPorTurmaRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.perguntasAutoralRepository = perguntasAutoralRepository ?? throw new ArgumentNullException(nameof(perguntasAutoralRepository));
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
        }

        public async Task<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaDto> Handle(ObterRelatorioSondagemPortuguesCapLeituraPorTurmaQuery request, CancellationToken cancellationToken)
        {
            var relatorio = new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaDto();

            var periodo = await ObterPeriodoSondagem(request.Bimestre, request.Semestre);
            var turma = await mediator.Send(new ObterTurmaSondagemEolPorCodigoQuery(request.TurmaCodigo));

            MontarCabecalho(relatorio, request.Dre, request.Ue, request.TurmaAno.ToString(), turma.Nome, request.AnoLetivo, periodo.Descricao, request.Usuario.CodigoRf, request.Usuario.Nome);

            var semestre = ObterSemestrePeriodo(request);
            var periodoCompleto = await ObterDatasPeriodoFixoAnual(0, semestre, request.AnoLetivo);

            var alunosDaTurma = await mediator.Send(new ObterAlunosPorTurmaDataSituacaoMatriculaQuery(request.TurmaCodigo, periodoCompleto.PeriodoFim, periodoCompleto.PeriodoInicio), cancellationToken);
            if (alunosDaTurma == null || !alunosDaTurma.Any())
                throw new NegocioException("Não foi possível localizar os alunos da turma.");

            var perguntas = await perguntasAutoralRepository.ObterPerguntasPorGrupo(GrupoSondagemEnum.CapacidadeLeitura, ComponenteCurricularSondagemEnum.Portugues);

            ObterPerguntas(relatorio, perguntas);

            var dados = await relatorioSondagemPortuguesPorTurmaRepository.ObterPorFiltros(GrupoSondagemEnum.CapacidadeLeitura.Name(), ComponenteCurricularSondagemEnum.Portugues.Name(), periodo.Id, request.AnoLetivo, request.TurmaCodigo.ToString());

            ObterLinhas(relatorio, dados, alunosDaTurma);
            var consideraNovaOpcaoRespostaSemPreenchimento = await mediator.Send(new UtilizarNovaOpcaoRespostaSemPreenchimentoQuery(request.Semestre,request.Bimestre,request.AnoLetivo), cancellationToken);
            GerarGrafico(relatorio, alunosDaTurma.Count(),consideraNovaOpcaoRespostaSemPreenchimento);

            return relatorio;
        }

        private static int ObterSemestrePeriodo(ObterRelatorioSondagemPortuguesCapLeituraPorTurmaQuery filtros)
        {
            if (filtros.Bimestre == 0) return filtros.Semestre;
            return filtros.Bimestre <= SEGUNDO_BIMESTRE ? PRIMEIRO_SEMESTRE : SEGUNDO_SEMESTRE;
        }

        private async Task<PeriodoCompletoSondagemDto> ObterDatasPeriodoFixoAnual(int bimestre, int semestre, int anoLetivo)
        {
            if (bimestre != 0)
                return await mediator.Send(new ObterDatasPeriodoSondagemPorBimestreAnoLetivoQuery(bimestre, anoLetivo));
            return await mediator.Send(new ObterDatasPeriodoSondagemPorSemestreAnoLetivoQuery(semestre, anoLetivo));
        }

        private async Task<PeriodoSondagem> ObterPeriodoSondagem(int bimestre, int semestre)
        {
            if (bimestre != 0)
                return await mediator.Send(new ObterPeriodoPorTipoQuery(bimestre, TipoPeriodoSondagem.Bimestre));
            return await mediator.Send(new ObterPeriodoPorTipoQuery(semestre, TipoPeriodoSondagem.Semestre));
        }

        private void GerarGrafico(RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaDto relatorio, int qtdAlunos, bool consideraNovaOpcaoRespostaSemPreenchimento)
        {
            foreach (var ordem in relatorio.Cabecalho.Ordens)
            {
                foreach (var pergunta in relatorio.Cabecalho.Perguntas)
                {
                    var legendas = new List<GraficoBarrasLegendaDto>();
                    var grafico = new GraficoBarrasVerticalDto(420, $"{ordem.Descricao} - {pergunta.Nome}");

                    var respostas = relatorio.Planilha.Linhas
                        .SelectMany(l => l.OrdensRespostas.Where(or => or.OrdemId == ordem?.Id && or.PerguntaId == pergunta?.Id && !string.IsNullOrEmpty(or.Resposta)))
                        .GroupBy(b => b.Resposta).OrderByDescending(a => a.Key.StartsWith("Adequada"));

                    var chaveIndex = 0;
                    var chave = string.Empty;
                    var qtdSemPreenchimento = 0;

                    foreach (var resposta in respostas.Where(a => !string.IsNullOrEmpty(a.Key)))
                    {
                        chave = lstChaves[chaveIndex++].ToString();

                        legendas.Add(new GraficoBarrasLegendaDto()
                        {
                            Chave = chave,
                            Valor = resposta.Key
                        });

                        var qntRespostas = resposta.Count();
                        grafico.EixosX.Add(new GraficoBarrasVerticalEixoXDto(qntRespostas, chave));
                    }

                    var totalRespostas = (int)grafico.EixosX.Sum(e => e.Valor);
                    qtdSemPreenchimento = qtdAlunos - totalRespostas;

                    if (!consideraNovaOpcaoRespostaSemPreenchimento)
                    {
                        if (qtdSemPreenchimento > 0)
                        {
                            chave = lstChaves[chaveIndex++].ToString();

                            legendas.Add(new GraficoBarrasLegendaDto()
                            {
                                Chave = chave,
                                Valor = "Sem preenchimento"
                            });

                            grafico.EixosX.Add(new GraficoBarrasVerticalEixoXDto(qtdSemPreenchimento, chave));
                        }
                    }

                    var valorMaximoEixo = grafico.EixosX.Any() ? grafico.EixosX.Max(a => int.Parse(a.Valor.ToString())) : 0;

                    grafico.EixoYConfiguracao = new GraficoBarrasVerticalEixoYDto(350, "Quantidade Alunos", valorMaximoEixo.ArredondaParaProximaDezena(), 10);
                    grafico.Legendas = legendas;

                    relatorio.GraficosBarras.Add(grafico);
                }
            }
        }

        private static void ObterLinhas(RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaDto relatorio, IEnumerable<SondagemAutoralPorAlunoDto> dados, IEnumerable<Aluno> alunosEol)
        {
            var alunosAgrupados = dados.GroupBy(x => x.CodigoAluno);

            relatorio.Planilha = new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPlanilhaDto();

            relatorio.Planilha.Linhas.AddRange(alunosEol.Select(alunoRetorno =>
            {
                var itemRelatorio = new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPlanilhaLinhasDto();
                var aluno = new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaAlunoDto()
                {
                    Codigo = alunoRetorno.CodigoAluno,
                    DataSituacao = alunoRetorno.DataSituacao.ToString("dd/MM/yyyy"),
                    Nome = alunoRetorno.ObterNomeParaRelatorioSondagem(),
                    SituacaoMatricula = alunoRetorno.SituacaoMatricula
                };

                itemRelatorio.Aluno = aluno;

                itemRelatorio.OrdensRespostas = new List<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto>();
                var alunoRespostas = alunosAgrupados.FirstOrDefault(x => x.Key == aluno.Codigo)?.ToList();

                if (alunoRespostas != null && alunoRespostas.Any())
                {
                    itemRelatorio.OrdensRespostas.AddRange(alunoRespostas.Select(aluno =>
                    {
                        var itemResposta = new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto()
                        {
                            OrdemId = aluno.OrdemId,
                            PerguntaId = aluno.PerguntaId,
                            Resposta = aluno.RespostaDescricao
                        };

                        return itemResposta;
                    }));
                }

                return itemRelatorio;
            }));

            if (relatorio.Planilha.Linhas != null && relatorio.Planilha.Linhas.Any())
                relatorio.Planilha.Linhas = relatorio.Planilha.Linhas.OrderBy(a => a.Aluno.Nome).ToList();
        }

        private static void ObterPerguntas(RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaDto relatorio, IEnumerable<PerguntasOrdemGrupoAutoralDto> perguntas)
        {
            if (perguntas != null && perguntas.Any())
            {
                relatorio.Cabecalho.Perguntas = new List<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPerguntaDto>();
                relatorio.Cabecalho.Ordens = new List<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemDto>();
            }

            var ordensAgrupado = perguntas.GroupBy(p => new { p.OrdemId, p.Ordem });
            var perguntasAgrupado = perguntas.GroupBy(p => new { p.PerguntaId, p.Pergunta });

            relatorio.Cabecalho.Ordens.AddRange(ordensAgrupado.Select(o => new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemDto()
            {
                Id = o.Key.OrdemId,
                Descricao = o.Key.Ordem
            }));

            relatorio.Cabecalho.Perguntas.AddRange(perguntasAgrupado.Select(o => new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPerguntaDto()
            {
                Id = o.Key.PerguntaId,
                Nome = o.Key.Pergunta
            }));
        }

        private static void MontarCabecalho(RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaDto relatorio, Dre dre, Ue ue, string anoTurma, string turmaNome, int anoLetivo, string periodo, string codigoRf, string usuario)
        {
            relatorio.Cabecalho = new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaCabecalhoDto()
            {
                Ano = anoTurma,
                AnoLetivo = anoLetivo,
                ComponenteCurricular = "Português",
                DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                Dre = dre != null ? dre.Abreviacao : "Todas",
                Periodo = periodo,
                Proficiencia = "IAD - Capacidade de Leitura",
                Rf = codigoRf,
                Turma = turmaNome,
                Ue = ue != null ? ue.NomeComTipoEscola : "Todas",
                Usuario = usuario,
            };
        }
    }
}
