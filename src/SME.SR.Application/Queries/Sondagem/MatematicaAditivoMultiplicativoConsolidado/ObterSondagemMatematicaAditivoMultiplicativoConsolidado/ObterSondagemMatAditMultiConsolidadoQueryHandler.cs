using MediatR;
using SME.SR.Data;
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
    public class ObterSondagemMatAditMultiConsolidadoQueryHandler : IRequestHandler<ObterSondagemMatAditMultiConsolidadoQuery, RelatorioSondagemComponentesMatematicaAditMulConsolidadoDto>
    {
        private readonly IMathPoolCARepository mathPoolCARepository;
        private readonly IMathPoolCMRepository mathPoolCMRepository;
        private readonly IPerguntasAutoralRepository perguntasAutoralRepository;
        private readonly ISondagemAutoralRepository sondagemAutoralRepository;
        private readonly IPerguntasAditMultiNumRepository perguntasAditMultiNumRepository;
        private readonly IMediator mediator;

        private readonly char[] lstChaves = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private const int ANO_LETIVO_DOIS_MIL_VINTE_DOIS = 2022;

        public ObterSondagemMatAditMultiConsolidadoQueryHandler(IMathPoolCARepository mathPoolCARepository, IMathPoolCMRepository mathPoolCMRepository,
            IPerguntasAditMultiNumRepository perguntasAditMultiNumRepository, ISondagemAutoralRepository sondagemAutoralRepository,IMediator mediator)
        {
            this.mathPoolCARepository = mathPoolCARepository ?? throw new ArgumentNullException(nameof(mathPoolCARepository)); ;
            this.mathPoolCMRepository = mathPoolCMRepository ?? throw new ArgumentNullException(nameof(mathPoolCMRepository)); ;
            this.perguntasAditMultiNumRepository = perguntasAditMultiNumRepository ?? throw new ArgumentNullException(nameof(perguntasAditMultiNumRepository));
            this.sondagemAutoralRepository = sondagemAutoralRepository ?? throw new ArgumentNullException(nameof(sondagemAutoralRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RelatorioSondagemComponentesMatematicaAditMulConsolidadoDto> Handle(ObterSondagemMatAditMultiConsolidadoQuery request, CancellationToken cancellationToken)
        {
            var consideraNovaOpcaoRespostaSemPreenchimento = await mediator.Send(new UtilizarNovaOpcaoRespostaSemPreenchimentoQuery(request.Semestre,request.Bimestre,request.AnoLetivo), cancellationToken);
            var relatorio = new RelatorioSondagemComponentesMatematicaAditMulConsolidadoDto();
            var respostas = new List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntasRespostasDto>();
            var perguntas = new List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntaDto>();

            MontarPerguntas(perguntas);
            MontarCabecalho(relatorio, request.Proficiencia, request.Dre, request.Ue, request.TurmaAno.ToString(), request.AnoLetivo, request.Semestre, request.Bimestre, request.Usuario.CodigoRf, request.Usuario.Nome);

            var qtdAlunos = 0;

            if (request.AnoLetivo < ANO_LETIVO_DOIS_MIL_VINTE_DOIS)
            {
                if (request.Proficiencia == ProficienciaSondagemEnum.CampoAditivo)
                {
                    var listaAlunos = await mathPoolCARepository.ObterPorFiltros(request.Dre?.Codigo, request.Ue?.Codigo, request.TurmaAno, request.AnoLetivo, request.Semestre);

                    qtdAlunos = listaAlunos.DistinctBy(a => a.AlunoEolCode).Count();

                    var ordem1Ideia = listaAlunos.GroupBy(fu => fu.Ordem1Ideia);

                    var ordem1Resultado = listaAlunos.GroupBy(fu => fu.Ordem1Resultado);

                    var ordem2Ideia = listaAlunos.GroupBy(fu => fu.Ordem2Ideia);

                    var ordem2Resultado = listaAlunos.GroupBy(fu => fu.Ordem2Resultado);

                    if (request.TurmaAno != 2)
                    {
                        var ordem3Ideia = listaAlunos.GroupBy(fu => fu.Ordem3Ideia);

                        var ordem3Resultado = listaAlunos.GroupBy(fu => fu.Ordem3Resultado);

                        AdicionarOrdem(request.Proficiencia, request.TurmaAno, ordem3Ideia, ordem3Resultado, ordem: 3, respostas, request.QuantidadeTotalAlunos);

                        if (request.TurmaAno != 1 && request.TurmaAno != 3)
                        {
                            var ordem4Ideia = listaAlunos.GroupBy(fu => fu.Ordem4Ideia);
                            var ordem4Resultado = listaAlunos.GroupBy(fu => fu.Ordem4Resultado);
                            AdicionarOrdem(request.Proficiencia, request.TurmaAno, ordem4Ideia, ordem4Resultado, ordem: 4, respostas, request.QuantidadeTotalAlunos);

                        }
                    }

                    AdicionarOrdem(request.Proficiencia, request.TurmaAno, ordem1Ideia, ordem1Resultado, ordem: 1, respostas, request.QuantidadeTotalAlunos);
                    AdicionarOrdem(request.Proficiencia, request.TurmaAno, ordem2Ideia, ordem2Resultado, ordem: 2, respostas, request.QuantidadeTotalAlunos);

                }
                else
                {
                    var listaAlunos = await mathPoolCMRepository.ObterPorFiltros(request.Dre?.Codigo, request.Ue?.Codigo, request.TurmaAno, request.AnoLetivo, request.Semestre);

                    qtdAlunos = listaAlunos.Count(a => !(string.IsNullOrEmpty(a.Ordem3Ideia) && string.IsNullOrWhiteSpace(a.Ordem3Resultado)));

                    if (request.TurmaAno == 2)
                    {
                        var ordem3Ideia = listaAlunos.GroupBy(fu => fu.Ordem3Ideia);

                        var ordem3Resultado = listaAlunos.GroupBy(fu => fu.Ordem3Resultado);

                        AdicionarOrdem(request.Proficiencia, request.TurmaAno, ordem3Ideia, ordem3Resultado, ordem: 3, respostas, request.QuantidadeTotalAlunos);
                    }
                    else
                    {
                        if (request.TurmaAno == 3)
                        {
                            var ordem4Ideia = listaAlunos.GroupBy(fu => fu.Ordem4Ideia);

                            var ordem4Resultado = listaAlunos.GroupBy(fu => fu.Ordem4Resultado);

                            AdicionarOrdem(request.Proficiencia, request.TurmaAno, ordem4Ideia, ordem4Resultado, ordem: 4, respostas, request.QuantidadeTotalAlunos);
                        }

                        var ordem5Ideia = listaAlunos.GroupBy(fu => fu.Ordem5Ideia);

                        var ordem5Resultado = listaAlunos.GroupBy(fu => fu.Ordem5Resultado);

                        AdicionarOrdem(request.Proficiencia, request.TurmaAno, ordem5Ideia, ordem5Resultado, ordem: 5, respostas, request.QuantidadeTotalAlunos);

                        if (request.TurmaAno != 3)
                        {
                            var ordem6Ideia = listaAlunos.GroupBy(fu => fu.Ordem6Ideia);

                            var ordem6Resultado = listaAlunos.GroupBy(fu => fu.Ordem6Resultado);

                            AdicionarOrdem(request.Proficiencia, request.TurmaAno, ordem6Ideia, ordem6Resultado, ordem: 6, respostas, request.QuantidadeTotalAlunos);

                            var ordem7Ideia = listaAlunos.GroupBy(fu => fu.Ordem7Ideia);

                            var ordem7Resultado = listaAlunos.GroupBy(fu => fu.Ordem6Resultado);

                            AdicionarOrdem(request.Proficiencia, request.TurmaAno, ordem7Ideia, ordem7Resultado, ordem: 7, respostas, request.QuantidadeTotalAlunos);
                        }

                        if (request.TurmaAno != 3 && request.TurmaAno != 4)
                        {
                            var ordem8Ideia = listaAlunos.GroupBy(fu => fu.Ordem8Ideia);

                            var ordem8Resultado = listaAlunos.GroupBy(fu => fu.Ordem8Resultado);

                            AdicionarOrdem(request.Proficiencia, request.TurmaAno, ordem8Ideia, ordem8Resultado, ordem: 8, respostas, request.QuantidadeTotalAlunos);
                        }
                    }
                }

                if (respostas.Any())
                {
                    respostas.ForEach(resposta => resposta.Respostas = resposta.Respostas.OrderBy(r => r.Resposta).ToList());
                    relatorio.PerguntasRespostas = respostas.OrderBy(r => r.Ordem).ToList();
                }
            }
            else
            {
                var listaPerguntasOrdem = await perguntasAditMultiNumRepository.ObterPerguntasOrdem(request.TurmaAno, request.AnoLetivo, ComponenteCurricularSondagemEnum.Matematica, request.Proficiencia);
                listaPerguntasOrdem = listaPerguntasOrdem.DistinctBy(lp => lp.Id).ToList();

                var listaAlunos = await sondagemAutoralRepository.ObterPorFiltros(request.Dre?.Codigo, request.Ue?.Codigo, string.Empty, string.Empty, request.Bimestre, request.TurmaAno, request.AnoLetivo, ComponenteCurricularSondagemEnum.Matematica);

                qtdAlunos = listaAlunos.DistinctBy(a => a.CodigoAluno).Count();
                var ordem = 1;
                foreach (var perguntaOrdem in listaPerguntasOrdem.Where(lpo => lpo.PerguntaId == null))
                {
                    var perguntaFilho = listaPerguntasOrdem.Where(lpo => lpo.PerguntaId == perguntaOrdem.Id);

                    var perguntasRespostas = new RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntasRespostasDto();

                    perguntasRespostas.Ordem = $"ORDEM {ordem} - {perguntaOrdem.Pergunta}";

                    foreach (var pergunta in perguntaFilho)
                    {
                        var respostasPergunta = await perguntasAditMultiNumRepository.ObterRespostasDaPergunta(pergunta.Id);

                        relatorio.Perguntas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntaDto()
                        {
                            PerguntaId = pergunta.Id,
                            Descricao = pergunta.Pergunta
                        });

                        foreach (var resposta in respostasPergunta)
                        {
                            var alunosQuantidade = listaAlunos.Count(l => l.RespostaId == resposta.RespostaId && l.PerguntaId == pergunta.Id);
                            perguntasRespostas.Respostas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto()
                            {
                                PerguntaNovaId = pergunta.Id,
                                Pergunta = pergunta.Pergunta,
                                PerguntaId = pergunta.Pergunta.Equals("Ideia") ? 1 : 2,
                                Resposta = resposta.Resposta,
                                AlunosQuantidade = alunosQuantidade,
                                AlunosPercentual = ((double)alunosQuantidade / request.QuantidadeTotalAlunos) * 100
                            });;
                        }
                    }

                    relatorio.PerguntasRespostas.Add(perguntasRespostas);
                    ordem++;
                }
            };

            if (perguntas.Any())
                relatorio.Perguntas = perguntas;

            if (request.AnoLetivo < 2022)
                TrataAlunosQueNaoResponderam(relatorio, qtdAlunos,consideraNovaOpcaoRespostaSemPreenchimento);
            else
                TrataAlunosQueNaoResponderam2022(relatorio, qtdAlunos, consideraNovaOpcaoRespostaSemPreenchimento);

            GerarGrafico(relatorio, qtdAlunos, consideraNovaOpcaoRespostaSemPreenchimento);

            return relatorio;
        }




        private void GerarGrafico(RelatorioSondagemComponentesMatematicaAditMulConsolidadoDto relatorio, int qtdAlunos,
            bool consideraNovaOpcaoRespostaSemPreenchimento)
        {
            var ordens = relatorio.PerguntasRespostas.Select(o => o.Ordem);

            foreach (var ordem in ordens)
            {
                foreach (var pergunta in relatorio.Perguntas)
                {
                    var legendas = new List<GraficoBarrasLegendaDto>();
                    var grafico = new GraficoBarrasVerticalDto(420, $"{ordem} - {pergunta.Descricao}");

                    var respostas = relatorio.PerguntasRespostas
                        .FirstOrDefault(o => o.Ordem == ordem)
                        ?.Respostas.Where(p => p.PerguntaId == pergunta.Id && !string.IsNullOrEmpty(p.Resposta))
                        .GroupBy(b => b.Resposta).OrderBy(a => a.Key);

                    var chaveIndex = 0;
                    var chave = string.Empty;
                    var qtdSemPreenchimento = 0;

                    if (respostas != null)
                        foreach (var resposta in respostas.Where(a => !string.IsNullOrEmpty(a.Key)))
                        {
                            chave = lstChaves[chaveIndex++].ToString();

                            legendas.Add(new GraficoBarrasLegendaDto()
                            {
                                Chave = chave,
                                Valor = resposta.Key
                            });

                            var qntRespostas = resposta.Sum(r => r.AlunosQuantidade);
                            grafico.EixosX.Add(new GraficoBarrasVerticalEixoXDto((decimal)qntRespostas, chave));
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

        private static void MontarPerguntas(List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntaDto> perguntas)
        {
            perguntas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntaDto()
            {
                Descricao = "Ideia",
                Id = 1
            });

            perguntas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntaDto()
            {
                Descricao = "Resultado",
                Id = 2
            });
        }

        private static void TrataAlunosQueNaoResponderam(RelatorioSondagemComponentesMatematicaAditMulConsolidadoDto relatorio, int quantidadeTotalAlunos,bool consideraNovaOpcaoRespostaSemPreenchimento)
        {
            if (consideraNovaOpcaoRespostaSemPreenchimento) return;
            foreach (var perguntaResposta in relatorio.PerguntasRespostas)
            {
                var qntDeAlunosPreencheuIdeia = perguntaResposta.Respostas?.Where(p => p.PerguntaId == 1).Sum(a => a.AlunosQuantidade) ?? 0;
                var qntDeAlunosPreencheuResultado = perguntaResposta.Respostas?.Where(p => p.PerguntaId == 2).Sum(a => a.AlunosQuantidade) ?? 0;

                var diferencaPreencheuNaoIdeia = Math.Max(quantidadeTotalAlunos - qntDeAlunosPreencheuIdeia, 0);
                var diferencaPreencheuNaoResultado = Math.Max(quantidadeTotalAlunos - qntDeAlunosPreencheuResultado, 0);

                var percentualNaoPreencheuIdeia = (diferencaPreencheuNaoIdeia / quantidadeTotalAlunos) * 100;
                var percentualNaoPreencheuResultado = (diferencaPreencheuNaoResultado / quantidadeTotalAlunos) * 100;

                perguntaResposta.Respostas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto()
                {
                    PerguntaId = 1,
                    Resposta = "Sem preenchimento",
                    AlunosQuantidade = diferencaPreencheuNaoIdeia,
                    AlunosPercentual = percentualNaoPreencheuIdeia
                });

                perguntaResposta.Respostas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto()
                {
                    PerguntaId = 2,
                    Resposta = "Sem preenchimento",
                    AlunosQuantidade = diferencaPreencheuNaoResultado,
                    AlunosPercentual = percentualNaoPreencheuResultado
                });
            }
        }

        private static void TrataAlunosQueNaoResponderam2022(RelatorioSondagemComponentesMatematicaAditMulConsolidadoDto relatorio, int quantidadeTotalAlunos,
            bool consideraNovaOpcaoRespostaSemPreenchimento)
        {
            foreach (var perguntaResposta in relatorio.PerguntasRespostas)
            {
                var contador = 0;
                foreach (var pergunta in relatorio.Perguntas.OrderBy(p => p.Descricao))
                {
                    if (contador == 0)
                    {
                        var resposta = perguntaResposta.Respostas?.FirstOrDefault(p => p.Pergunta.Equals("Ideia"));
                        var qntDeAlunosPreencheuIdeia = perguntaResposta.Respostas?.Where(p => p.Pergunta.Equals("Ideia")).Sum(a => a.AlunosQuantidade) ?? 0;
                        var diferencaPreencheuNaoIdeia = quantidadeTotalAlunos - qntDeAlunosPreencheuIdeia;
                        var percentualNaoPreencheuIdeia = (diferencaPreencheuNaoIdeia / quantidadeTotalAlunos) * 100;

                        if (!consideraNovaOpcaoRespostaSemPreenchimento)
                        {
                            perguntaResposta.Respostas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto()
                            {
                                PerguntaId = 1,
                                Pergunta = "Ideia",
                                PerguntaNovaId = resposta != null ? resposta.PerguntaNovaId: "",
                                Resposta = "Sem preenchimento",
                                AlunosQuantidade = diferencaPreencheuNaoIdeia,
                                AlunosPercentual = percentualNaoPreencheuIdeia
                            }); 
                        }
                    }
                    else if (contador > 0)
                    {
                        var resposta = perguntaResposta.Respostas?.FirstOrDefault(p => p.Pergunta.Equals("Resultado"));
                        var qntDeAlunosPreencheuResultado = perguntaResposta.Respostas?.Where(p => p.Pergunta.Equals("Resultado")).Sum(a => a.AlunosQuantidade) ?? 0;
                        var diferencaPreencheuNaoResultado = quantidadeTotalAlunos - qntDeAlunosPreencheuResultado;
                        var percentualNaoPreencheuResultado = (diferencaPreencheuNaoResultado / quantidadeTotalAlunos) * 100;

                        if (!consideraNovaOpcaoRespostaSemPreenchimento)
                        {
                            perguntaResposta.Respostas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto()
                            {
                                PerguntaId = 2,
                                Pergunta = "Resultado",
                                PerguntaNovaId = resposta != null ? resposta.PerguntaNovaId : "",
                                Resposta = "Sem preenchimento",
                                AlunosQuantidade = diferencaPreencheuNaoResultado,
                                AlunosPercentual = percentualNaoPreencheuResultado
                            });
                        }
                    }
                    contador++;
                }

            }
        }

        private static void MontarCabecalho(RelatorioSondagemComponentesMatematicaAditMulConsolidadoDto relatorio, ProficienciaSondagemEnum proficiencia, Dre dre, Ue ue, string anoTurma, int anoLetivo, int semestre, int bimestre, string rf, string usuario)
        {
            relatorio.Ano = anoTurma;
            relatorio.AnoLetivo = anoLetivo;
            relatorio.ComponenteCurricular = "Matemática";
            relatorio.DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy");
            relatorio.Dre = dre != null ? dre.Abreviacao : "Todas";
            relatorio.Periodo = anoLetivo >= 2022 && bimestre > 0 ? $"{bimestre}º Bimestre" : $"{semestre}º Semestre";
            relatorio.Proficiencia = proficiencia.Name();
            relatorio.RF = rf;
            relatorio.Turma = "Todas";
            relatorio.Ue = ue != null ? ue.NomeComTipoEscola : "Todas";
            relatorio.Usuario = usuario;
        }

        private void AdicionarOrdem(ProficienciaSondagemEnum proficiencia, int anoTurma, IEnumerable<IGrouping<string, MathPoolCA>> agrupamentoIdeia, IEnumerable<IGrouping<string, MathPoolCA>> agrupamentoResultado, int ordem, List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntasRespostasDto> respostas, int totalAlunosGeral)
        {
            var lstRespostas = new List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto>();

            AdicionarRespostasAgrupamento(lstRespostas, agrupamentoIdeia, 1, totalAlunosGeral);
            AdicionarRespostasAgrupamento(lstRespostas, agrupamentoResultado, 2, totalAlunosGeral);

            respostas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntasRespostasDto()
            {
                Ordem = ObterTituloOrdem(proficiencia, anoTurma, ordem),
                Respostas = lstRespostas
            });
        }

        private static void AdicionarOrdem(ProficienciaSondagemEnum proficiencia, int anoTurma, IEnumerable<IGrouping<string, MathPoolCM>> agrupamentoIdeia, IEnumerable<IGrouping<string, MathPoolCM>> agrupamentoResultado, int ordem, List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntasRespostasDto> respostas, int totalAlunosGeral)
        {
            var lstRespostas = new List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto>();

            AdicionarRespostasAgrupamento(lstRespostas, agrupamentoIdeia, 1, totalAlunosGeral);
            AdicionarRespostasAgrupamento(lstRespostas, agrupamentoResultado, 2, totalAlunosGeral);

            respostas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntasRespostasDto()
            {
                Ordem = ObterTituloOrdem(proficiencia, anoTurma, ordem),
                Respostas = lstRespostas
            });
        }

        private static void AdicionarRespostasAgrupamento(List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto> lstRespostas, IEnumerable<IGrouping<string, MathPoolCM>> agrupamento, int perguntaId, int totalAlunosGeral)
        {
            var respIdeias = ObterRespostas(agrupamento, perguntaId, totalAlunosGeral);

            if (respIdeias != null && respIdeias.Any())
                lstRespostas.AddRange(respIdeias);
        }

        private void AdicionarRespostasAgrupamento(List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto> lstRespostas, IEnumerable<IGrouping<string, MathPoolCA>> agrupamento, int perguntaId, int totalAlunosGeral)
        {
            var respIdeias = ObterRespostas(agrupamento, perguntaId, totalAlunosGeral);

            if (respIdeias != null && respIdeias.Any())
                lstRespostas.AddRange(respIdeias);
        }

        private static List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto> ObterRespostas(IEnumerable<IGrouping<string, MathPoolCA>> agrupamento, int perguntaId, int totalAlunosGeral)
        {
            var agrupamentosComValor = agrupamento.Where(a => a.Key != null && !a.Key.Trim().Equals(""));

            var respostas = (from item in agrupamentosComValor let respostaDesc = ConverteTextoPollMatematica(item.Key) 
                select new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto() 
                    { PerguntaId = perguntaId, Resposta = respostaDesc, AlunosQuantidade = item.Count(), AlunosPercentual = ((double)item.Count() / totalAlunosGeral) * 100 }).ToList();

            return respostas.Any() ? respostas : null;
        }

        private static List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto> ObterRespostas(IEnumerable<IGrouping<string, MathPoolCM>> agrupamento, int perguntaId, int totalAlunosGeral)
        {
            var agrupamentosComValor = agrupamento.Where(a => a.Key != null && !a.Key.Trim().Equals(""));

            var respostas = (from item in agrupamentosComValor let respostaDesc = 
                ConverteTextoPollMatematica(item.Key) select new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto() 
                { PerguntaId = perguntaId, Resposta = respostaDesc, AlunosQuantidade = item.Count(), AlunosPercentual = ((double)item.Count() / totalAlunosGeral) * 100 }).ToList();

            return respostas.Any() ? respostas : null;
        }

        private static string ConverteTextoPollMatematica(string texto)
        {
            return texto switch
            {
                "A" => "Acertou",
                "E" => "Errou",
                "NR" => "Não Resolveu",
                _ => ""
            };
        }

        private static string ObterTituloOrdem(ProficienciaSondagemEnum proficiencia, int anoTurma, int ordem)
        {
            var ordemTitulo = string.Empty;

            switch (anoTurma)
            {
                case 1:
                    ordemTitulo = proficiencia switch
                    {
                        ProficienciaSondagemEnum.CampoAditivo => "COMPOSIÇÃO",
                        _ => string.Empty
                    };
                    break;
                case 2:
                    ordemTitulo = proficiencia switch
                    {
                        ProficienciaSondagemEnum.CampoAditivo => ordem switch
                        {
                            1 => "COMPOSIÇÃO",
                            2 => "TRANSFORMAÇÃO",
                            _ => string.Empty
                        },
                        ProficienciaSondagemEnum.CampoMultiplicativo => ordem switch
                        {
                            3 => "PROPORCIONALIDADE",
                            _ => string.Empty
                        },
                        _ => ordemTitulo
                    };
                    break;
                case 3:
                    ordemTitulo = proficiencia switch
                    {
                        ProficienciaSondagemEnum.CampoAditivo => ordem switch
                        {
                            1 => "COMPOSIÇÃO",
                            2 => "TRANSFORMAÇÃO",
                            3 => "COMPARAÇÃO",
                            _ => string.Empty
                        },
                        ProficienciaSondagemEnum.CampoMultiplicativo => ordem switch
                        {
                            4 => "CONFIGURAÇÃO RETANGULAR",
                            5 => "PROPORCIONALIDADE",
                            _ => string.Empty
                        },
                        _ => ordemTitulo
                    };
                    break;
                case 4:
                    ordemTitulo = proficiencia switch
                    {
                        ProficienciaSondagemEnum.CampoAditivo => ordem switch
                        {
                            1 => "COMPOSIÇÃO",
                            2 => "TRANSFORMAÇÃO",
                            3 => "COMPOSIÇÃO DE TRANSF.",
                            4 => "COMPARAÇÃO",
                            _ => string.Empty
                        },
                        ProficienciaSondagemEnum.CampoMultiplicativo => ordem switch
                        {
                            5 => "CONFIGURAÇÃO RETANGULAR",
                            6 => "PROPORCIONALIDADE",
                            7 => "COMBINATÓRIA",
                            _ => string.Empty
                        },
                        _ => ordemTitulo
                    };
                    break;
                case 5:
                    ordemTitulo = proficiencia switch
                    {
                        ProficienciaSondagemEnum.CampoAditivo => ordem switch
                        {
                            1 => "COMPOSIÇÃO",
                            2 => "TRANSFORMAÇÃO",
                            3 => "COMPOSIÇÃO DE TRANSF.",
                            4 => "COMPARAÇÃO",
                            _ => string.Empty
                        },
                        ProficienciaSondagemEnum.CampoMultiplicativo => ordem switch
                        {
                            5 => "COMBINATÓRIA",
                            6 => "CONFIGURAÇÃO RETANGULAR",
                            7 => "PROPORCIONALIDADE",
                            8 => "MULTIPLICAÇÃO COMPARATIVA",
                            _ => string.Empty
                        },
                        _ => ordemTitulo
                    };
                    break;
                case 6:
                    ordemTitulo = proficiencia switch
                    {
                        ProficienciaSondagemEnum.CampoAditivo => ordem switch
                        {
                            1 => "COMPOSIÇÃO",
                            2 => "TRANSFORMAÇÃO",
                            3 => "COMPOSIÇÃO DE TRANSF.",
                            4 => "COMPARAÇÃO",
                            _ => string.Empty
                        },
                        ProficienciaSondagemEnum.CampoMultiplicativo => ordem switch
                        {
                            5 => "COMBINATÓRIA",
                            6 => "CONFIGURAÇÃO RETANGULAR",
                            7 => "PROPORCIONALIDADE",
                            8 => "MULTIPLICAÇÃO COMPARATIVA",
                            _ => string.Empty
                        },
                        _ => ordemTitulo
                    };
                    break;
                default:
                    break;

            }

            return $"ORDEM {ordem} - {ordemTitulo}";
        }
    }
}
