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

        private readonly char[] lstChaves = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        public ObterSondagemMatAditMultiConsolidadoQueryHandler(IMathPoolCARepository mathPoolCARepository, IMathPoolCMRepository mathPoolCMRepository,
            IPerguntasAditMultiNumRepository perguntasAditMultiNumRepository, ISondagemAutoralRepository sondagemAutoralRepository)
        {
            this.mathPoolCARepository = mathPoolCARepository ?? throw new ArgumentNullException(nameof(mathPoolCARepository)); ;
            this.mathPoolCMRepository = mathPoolCMRepository ?? throw new ArgumentNullException(nameof(mathPoolCMRepository)); ;
            this.perguntasAditMultiNumRepository = perguntasAditMultiNumRepository ?? throw new ArgumentNullException(nameof(perguntasAditMultiNumRepository));
            this.sondagemAutoralRepository = sondagemAutoralRepository ?? throw new ArgumentNullException(nameof(sondagemAutoralRepository));
        }

        public async Task<RelatorioSondagemComponentesMatematicaAditMulConsolidadoDto> Handle(ObterSondagemMatAditMultiConsolidadoQuery request, CancellationToken cancellationToken)
        {
            var relatorio = new RelatorioSondagemComponentesMatematicaAditMulConsolidadoDto();
            var respostas = new List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntasRespostasDto>();
            var perguntas = new List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntaDto>();

            MontarPerguntas(perguntas);
            MontarCabecalho(relatorio, request.Proficiencia, request.Dre, request.Ue, request.TurmaAno.ToString(), request.AnoLetivo, request.Semestre, request.Bimestre, request.Usuario.CodigoRf, request.Usuario.Nome);

            int qtdAlunos = 0;

            if (request.AnoLetivo < 2022)
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
                int ordem = 1;
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
                            var alunosQuantidade = listaAlunos.Where(l => l.RespostaId == resposta.RespostaId && l.PerguntaId == pergunta.Id).Count();
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
                TrataAlunosQueNaoResponderam(relatorio, qtdAlunos);
            else
                TrataAlunosQueNaoResponderam2022(relatorio, qtdAlunos);

            GerarGrafico(relatorio, qtdAlunos);

            return relatorio;
        }




        private void GerarGrafico(RelatorioSondagemComponentesMatematicaAditMulConsolidadoDto relatorio, int qtdAlunos)
        {
            var ordens = relatorio.PerguntasRespostas.Select(o => o.Ordem);

            foreach (var ordem in ordens)
            {
                foreach (var pergunta in relatorio.Perguntas)
                {
                    var legendas = new List<GraficoBarrasLegendaDto>();
                    var grafico = new GraficoBarrasVerticalDto(420, $"{ordem} - {pergunta.Descricao}");

                    var respostas = relatorio.PerguntasRespostas
                        .FirstOrDefault(o => o.Ordem == ordem).Respostas.Where(p => p.PerguntaId == pergunta.Id && !string.IsNullOrEmpty(p.Resposta))
                        .GroupBy(b => b.Resposta).OrderBy(a => a.Key);

                    int chaveIndex = 0;
                    string chave = String.Empty;
                    int qtdSemPreenchimento = 0;

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

                    var valorMaximoEixo = grafico.EixosX.Count() > 0 ? grafico.EixosX.Max(a => int.Parse(a.Valor.ToString())) : 0;

                    grafico.EixoYConfiguracao = new GraficoBarrasVerticalEixoYDto(350, "Quantidade Alunos", valorMaximoEixo.ArredondaParaProximaDezena(), 10);
                    grafico.Legendas = legendas;

                    relatorio.GraficosBarras.Add(grafico);
                }
            }
        }

        private void MontarPerguntas(List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntaDto> perguntas)
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

        private void TrataAlunosQueNaoResponderam(RelatorioSondagemComponentesMatematicaAditMulConsolidadoDto relatorio, int quantidadeTotalAlunos)
        {
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

        private void TrataAlunosQueNaoResponderam2022(RelatorioSondagemComponentesMatematicaAditMulConsolidadoDto relatorio, int quantidadeTotalAlunos)
        {
            foreach (var perguntaResposta in relatorio.PerguntasRespostas)
            {
                int contador = 0;
                foreach (var pergunta in relatorio.Perguntas.OrderBy(p => p.Descricao))
                {
                    if (contador == 0)
                    {
                        var resposta = perguntaResposta.Respostas?.FirstOrDefault(p => p.Pergunta.Equals("Ideia"));
                        var qntDeAlunosPreencheuIdeia = perguntaResposta.Respostas?.Where(p => p.Pergunta.Equals("Ideia")).Sum(a => a.AlunosQuantidade) ?? 0;
                        var diferencaPreencheuNaoIdeia = quantidadeTotalAlunos - qntDeAlunosPreencheuIdeia;
                        var percentualNaoPreencheuIdeia = (diferencaPreencheuNaoIdeia / quantidadeTotalAlunos) * 100;

                        perguntaResposta.Respostas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto()
                        {
                            PerguntaId = 1,
                            Pergunta = "Ideia",
                            PerguntaNovaId = resposta.PerguntaNovaId,
                            Resposta = "Sem preenchimento",
                            AlunosQuantidade = diferencaPreencheuNaoIdeia,
                            AlunosPercentual = percentualNaoPreencheuIdeia
                        }); 
                    }
                    else if (contador > 0)
                    {
                        var resposta = perguntaResposta.Respostas?.FirstOrDefault(p => p.Pergunta.Equals("Resultado"));
                        var qntDeAlunosPreencheuResultado = perguntaResposta.Respostas?.Where(p => p.Pergunta.Equals("Resultado")).Sum(a => a.AlunosQuantidade) ?? 0;
                        var diferencaPreencheuNaoResultado = quantidadeTotalAlunos - qntDeAlunosPreencheuResultado;
                        var percentualNaoPreencheuResultado = (diferencaPreencheuNaoResultado / quantidadeTotalAlunos) * 100;

                        perguntaResposta.Respostas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto()
                        {
                            PerguntaId = 2,
                            Pergunta = "Resultado",
                            PerguntaNovaId = resposta.PerguntaNovaId,
                            Resposta = "Sem preenchimento",
                            AlunosQuantidade = diferencaPreencheuNaoResultado,
                            AlunosPercentual = percentualNaoPreencheuResultado
                        });
                    }
                    contador++;
                }

            }
        }

        private void MontarCabecalho(RelatorioSondagemComponentesMatematicaAditMulConsolidadoDto relatorio, ProficienciaSondagemEnum proficiencia, Dre dre, Ue ue, string anoTurma, int anoLetivo, int semestre, int bimestre, string rf, string usuario)
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

        private void AdicionarOrdem(ProficienciaSondagemEnum proficiencia, int anoTurma, IEnumerable<IGrouping<string, MathPoolCM>> agrupamentoIdeia, IEnumerable<IGrouping<string, MathPoolCM>> agrupamentoResultado, int ordem, List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntasRespostasDto> respostas, int totalAlunosGeral)
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

        private void AdicionarRespostasAgrupamento(List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto> lstRespostas, IEnumerable<IGrouping<string, MathPoolCM>> agrupamento, int perguntaId, int totalAlunosGeral)
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

        private List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto> ObterRespostas(IEnumerable<IGrouping<string, MathPoolCA>> agrupamento, int perguntaId, int totalAlunosGeral)
        {
            var respostas = new List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto>();

            var agrupamentosComValor = agrupamento.Where(a => a.Key != null && !a.Key.Trim().Equals(""));

            foreach (var item in agrupamentosComValor)
            {
                var respostaDesc = ConverteTextoPollMatematica(item.Key);

                respostas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto()
                {
                    PerguntaId = perguntaId,
                    Resposta = respostaDesc,
                    AlunosQuantidade = item.Count(),
                    AlunosPercentual = ((double)item.Count() / totalAlunosGeral) * 100
                });
            }

            if (respostas.Any())
                return respostas;
            else
                return null;
        }

        private List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto> ObterRespostas(IEnumerable<IGrouping<string, MathPoolCM>> agrupamento, int perguntaId, int totalAlunosGeral)
        {
            var respostas = new List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto>();

            var agrupamentosComValor = agrupamento.Where(a => a.Key != null && !a.Key.Trim().Equals(""));

            foreach (var item in agrupamentosComValor)
            {
                var respostaDesc = ConverteTextoPollMatematica(item.Key);

                respostas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto()
                {
                    PerguntaId = perguntaId,
                    Resposta = respostaDesc,
                    AlunosQuantidade = item.Count(),
                    AlunosPercentual = ((double)item.Count() / totalAlunosGeral) * 100
                });
            }

            if (respostas.Any())
                return respostas;
            else
                return null;
        }

        private string ConverteTextoPollMatematica(string texto)
        {
            switch (texto)
            {
                case "A":
                    return "Acertou";
                case "E":
                    return "Errou";
                case "NR":
                    return "Não Resolveu";
                default:
                    return "";
            }
        }

        private string ObterTituloOrdem(ProficienciaSondagemEnum proficiencia, int anoTurma, int ordem)
        {
            string ordemTitulo = string.Empty;

            switch (anoTurma)
            {
                case 1:
                    switch (proficiencia)
                    {
                        case ProficienciaSondagemEnum.CampoAditivo:
                            ordemTitulo = "COMPOSIÇÃO";
                            break;
                        default:
                            ordemTitulo = string.Empty;
                            break;
                    }
                    break;
                case 2:
                    switch (proficiencia)
                    {
                        case ProficienciaSondagemEnum.CampoAditivo:
                            switch (ordem)
                            {
                                case 1:
                                    ordemTitulo = "COMPOSIÇÃO";
                                    break;
                                case 2:
                                    ordemTitulo = "TRANSFORMAÇÃO";
                                    break;
                                default:
                                    ordemTitulo = string.Empty;
                                    break;
                            }
                            break;
                        case ProficienciaSondagemEnum.CampoMultiplicativo:
                            switch (ordem)
                            {
                                case 3:
                                    ordemTitulo = "PROPORCIONALIDADE";
                                    break;
                                default:
                                    ordemTitulo = string.Empty;
                                    break;
                            }
                            break;
                    }
                    break;
                case 3:
                    switch (proficiencia)
                    {
                        case ProficienciaSondagemEnum.CampoAditivo:
                            switch (ordem)
                            {
                                case 1:
                                    ordemTitulo = "COMPOSIÇÃO";
                                    break;
                                case 2:
                                    ordemTitulo = "TRANSFORMAÇÃO";
                                    break;
                                case 3:
                                    ordemTitulo = "COMPARAÇÃO";
                                    break;
                                default:
                                    ordemTitulo = string.Empty;
                                    break;
                            }
                            break;
                        case ProficienciaSondagemEnum.CampoMultiplicativo:
                            switch (ordem)
                            {
                                case 4:
                                    ordemTitulo = "CONFIGURAÇÃO RETANGULAR";
                                    break;
                                case 5:
                                    ordemTitulo = "PROPORCIONALIDADE";
                                    break;
                                default:
                                    ordemTitulo = string.Empty;
                                    break;
                            }
                            break;
                    }
                    break;
                case 4:
                    switch (proficiencia)
                    {
                        case ProficienciaSondagemEnum.CampoAditivo:
                            switch (ordem)
                            {
                                case 1:
                                    ordemTitulo = "COMPOSIÇÃO";
                                    break;
                                case 2:
                                    ordemTitulo = "TRANSFORMAÇÃO";
                                    break;
                                case 3:
                                    ordemTitulo = "COMPOSIÇÃO DE TRANSF.";
                                    break;
                                case 4:
                                    ordemTitulo = "COMPARAÇÃO";
                                    break;
                                default:
                                    ordemTitulo = string.Empty;
                                    break;
                            }
                            break;
                        case ProficienciaSondagemEnum.CampoMultiplicativo:
                            switch (ordem)
                            {
                                case 5:
                                    ordemTitulo = "CONFIGURAÇÃO RETANGULAR";
                                    break;
                                case 6:
                                    ordemTitulo = "PROPORCIONALIDADE";
                                    break;
                                case 7:
                                    ordemTitulo = "COMBINATÓRIA";
                                    break;
                                default:
                                    ordemTitulo = string.Empty;
                                    break;
                            }
                            break;
                    }
                    break;
                case 5:
                    switch (proficiencia)
                    {
                        case ProficienciaSondagemEnum.CampoAditivo:
                            switch (ordem)
                            {
                                case 1:
                                    ordemTitulo = "COMPOSIÇÃO";
                                    break;
                                case 2:
                                    ordemTitulo = "TRANSFORMAÇÃO";
                                    break;
                                case 3:
                                    ordemTitulo = "COMPOSIÇÃO DE TRANSF.";
                                    break;
                                case 4:
                                    ordemTitulo = "COMPARAÇÃO";
                                    break;
                                default:
                                    ordemTitulo = string.Empty;
                                    break;
                            }
                            break;
                        case ProficienciaSondagemEnum.CampoMultiplicativo:
                            switch (ordem)
                            {
                                case 5:
                                    ordemTitulo = "COMBINATÓRIA";
                                    break;
                                case 6:
                                    ordemTitulo = "CONFIGURAÇÃO RETANGULAR";
                                    break;
                                case 7:
                                    ordemTitulo = "PROPORCIONALIDADE";
                                    break;
                                case 8:
                                    ordemTitulo = "MULTIPLICAÇÃO COMPARATIVA";
                                    break;
                                default:
                                    ordemTitulo = string.Empty;
                                    break;
                            }
                            break;
                    }
                    break;
                case 6:
                    switch (proficiencia)
                    {
                        case ProficienciaSondagemEnum.CampoAditivo:
                            switch (ordem)
                            {
                                case 1:
                                    ordemTitulo = "COMPOSIÇÃO";
                                    break;
                                case 2:
                                    ordemTitulo = "TRANSFORMAÇÃO";
                                    break;
                                case 3:
                                    ordemTitulo = "COMPOSIÇÃO DE TRANSF.";
                                    break;
                                case 4:
                                    ordemTitulo = "COMPARAÇÃO";
                                    break;
                                default:
                                    ordemTitulo = string.Empty;
                                    break;
                            }
                            break;
                        case ProficienciaSondagemEnum.CampoMultiplicativo:
                            switch (ordem)
                            {
                                case 5:
                                    ordemTitulo = "COMBINATÓRIA";
                                    break;
                                case 6:
                                    ordemTitulo = "CONFIGURAÇÃO RETANGULAR";
                                    break;
                                case 7:
                                    ordemTitulo = "PROPORCIONALIDADE";
                                    break;
                                case 8:
                                    ordemTitulo = "MULTIPLICAÇÃO COMPARATIVA";
                                    break;
                                default:
                                    ordemTitulo = string.Empty;
                                    break;
                            }
                            break;
                    }
                    break;
                default:
                    break;

            }

            return $"ORDEM {ordem} - {ordemTitulo}";
        }
    }
}
