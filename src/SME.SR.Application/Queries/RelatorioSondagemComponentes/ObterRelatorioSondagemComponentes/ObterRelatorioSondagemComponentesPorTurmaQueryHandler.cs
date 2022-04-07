using MediatR;
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
    public class ObterRelatorioSondagemComponentesPorTurmaQueryHandler : IRequestHandler<ObterRelatorioSondagemComponentesPorTurmaQuery, RelatorioSondagemComponentesPorTurmaRelatorioDto>
    {
        private readonly IRelatorioSondagemComponentePorTurmaRepository relatorioSondagemComponentePorTurmaRepository;
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IMediator mediator;

        public ObterRelatorioSondagemComponentesPorTurmaQueryHandler(
            IRelatorioSondagemComponentePorTurmaRepository relatorioSondagemComponentePorTurmaRepository,
            IUsuarioRepository usuarioRepository,
            IMediator mediator)
        {
            this.relatorioSondagemComponentePorTurmaRepository = relatorioSondagemComponentePorTurmaRepository ?? throw new ArgumentNullException(nameof(relatorioSondagemComponentePorTurmaRepository));
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RelatorioSondagemComponentesPorTurmaRelatorioDto> Handle(ObterRelatorioSondagemComponentesPorTurmaQuery request, CancellationToken cancellationToken)
        {
            RelatorioSondagemComponentesPorTurmaCabecalhoDto cabecalho = await ObterCabecalho(request);
            RelatorioSondagemComponentesPorTurmaPlanilhaDto planilha;

            if (request.AnoLetivo < 2022)
                planilha = (int.Parse(request.Ano) >= 7) ? await ObterPlanilhaAutoral(request, cabecalho.Perguntas) : await ObterPlanilha(request);
            else
                planilha = (int.Parse(request.Ano) >= 4) ? await ObterPlanilhaPerguntasRespostasAutoral(request, cabecalho.Perguntas) : await ObterPlanilhaPerguntasRespostasProficiencia(request);

            var relatorio = new RelatorioSondagemComponentesPorTurmaRelatorioDto()
            {
                Cabecalho = cabecalho,
                Planilha = planilha
            };

            if (request.Proficiencia == ProficienciaSondagemEnum.CampoAditivo || request.Proficiencia == ProficienciaSondagemEnum.CampoMultiplicativo)
                GerarGraficosCamposAditivoMultiplicativo(relatorio, planilha.Linhas.Count);
            else if (request.Proficiencia == ProficienciaSondagemEnum.Numeros)
                GerarGraficoParaNumeros(relatorio);
            else
                GerarGraficoAutoral(relatorio);

            return relatorio;
        }

        private void GerarGraficosCamposAditivoMultiplicativo(RelatorioSondagemComponentesPorTurmaRelatorioDto relatorio, int qtdAlunos)
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

                    int chaveIndex = 0;
                    string chave = string.Empty;
                    int qtdSemPreenchimento = 0;

                    foreach (var resposta in respostas.Where(a => !string.IsNullOrEmpty(a.Key)))
                    {
                        chave = Constantes.ListaChavesGraficos[chaveIndex++].ToString();

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

                    if (qtdSemPreenchimento > 0)
                    {
                        chave = Constantes.ListaChavesGraficos[chaveIndex++].ToString();

                        legendas.Add(new GraficoBarrasLegendaDto()
                        {
                            Chave = chave,
                            Valor = "Sem preenchimento"
                        });

                        grafico.EixosX.Add(new GraficoBarrasVerticalEixoXDto(qtdSemPreenchimento, chave));
                    }

                    var valorMaximoEixo = grafico.EixosX.Count > 0 ? grafico.EixosX.Max(a => int.Parse(a.Valor.ToString())) : 0;

                    grafico.EixoYConfiguracao = new GraficoBarrasVerticalEixoYDto(350, "Quantidade Alunos", valorMaximoEixo.ArredondaParaProximaDezena(), 10);
                    grafico.Legendas = legendas;

                    relatorio.GraficosBarras.Add(grafico);
                }
            }
        }

        private void GerarGraficoParaNumeros(RelatorioSondagemComponentesPorTurmaRelatorioDto relatorio)
        {
            foreach (var pergunta in relatorio.Cabecalho.Perguntas)
            {
                var legendas = new List<GraficoBarrasLegendaDto>();
                var grafico = new GraficoBarrasVerticalDto(420, pergunta.Nome);

                var respostas = relatorio.Planilha.Linhas
                     .SelectMany(l => l.OrdensRespostas.Where(or => or.PerguntaId == pergunta?.Id && !string.IsNullOrEmpty(or.Resposta)))
                     .GroupBy(b => b.Resposta).OrderByDescending(a => a.Key.StartsWith("Escreve"));

                int chaveIndex = 0;
                string chave = string.Empty;

                foreach (var resposta in respostas.Where(a => !string.IsNullOrEmpty(a.Key)))
                {
                    chave = Constantes.ListaChavesGraficos[chaveIndex++].ToString();

                    legendas.Add(new GraficoBarrasLegendaDto()
                    {
                        Chave = chave,
                        Valor = resposta.Key
                    });

                    var qntRespostas = resposta.Count();
                    grafico.EixosX.Add(new GraficoBarrasVerticalEixoXDto(qntRespostas, chave));
                }

                var respostasNulasOuVazias = relatorio.Planilha.Linhas.SelectMany(l => l.OrdensRespostas
                    .Where(or => or.PerguntaId == pergunta?.Id && string.IsNullOrEmpty(or.Resposta))).GroupBy(b => b.Resposta);

                if (respostasNulasOuVazias.Any())
                {
                    var qntSemRespostas = 0;

                    foreach (var item in respostasNulasOuVazias)
                    {
                        qntSemRespostas += item.Count();
                    }

                    if (qntSemRespostas > 0)
                    {
                        chave = Constantes.ListaChavesGraficos[chaveIndex++].ToString();

                        legendas.Add(new GraficoBarrasLegendaDto()
                        {
                            Chave = chave,
                            Valor = "Sem preenchimento"
                        });

                        grafico.EixosX.Add(new GraficoBarrasVerticalEixoXDto(qntSemRespostas, chave));
                    }
                }

                var valorMaximoEixo = grafico.EixosX.Max(a => int.Parse(a.Valor.ToString()));

                grafico.EixoYConfiguracao = new GraficoBarrasVerticalEixoYDto(350, "Quantidade Alunos", valorMaximoEixo.ArredondaParaProximaDezena(), 10);
                grafico.Legendas = legendas;

                relatorio.GraficosBarras.Add(grafico);
            }
        }

        private void GerarGraficoAutoral(RelatorioSondagemComponentesPorTurmaRelatorioDto relatorio)
        {
            relatorio.GraficosBarras = new List<GraficoBarrasVerticalDto>();

            foreach (var pergunta in relatorio.Cabecalho.Perguntas)
            {
                var respostasPorPergunta = new List<RepostaTotalDto>();
                var grafico = new GraficoBarrasVerticalDto(420, pergunta.Nome);
                var legendas = new List<GraficoBarrasLegendaDto>();
                int chaveIndex = 0;

                foreach (var linha in relatorio.Planilha.Linhas)
                {
                    var resposta = linha.OrdensRespostas.Find(r => r.PerguntaId == pergunta.Id);

                    if (resposta != null)
                    {
                        var perguntaResposta = respostasPorPergunta.Find(pr => pr.Resposta == resposta.Resposta);

                        if (perguntaResposta != null)
                        {
                            perguntaResposta.Quantidade++;
                        }
                        else
                        {
                            respostasPorPergunta.Add(new RepostaTotalDto()
                            {
                                OrdenacaoResposta = string.IsNullOrEmpty(resposta.Resposta) ? 99 : resposta.OrdenacaoResposta,
                                Quantidade = 1,
                                Resposta = resposta.Resposta
                            });
                        }
                    }
                }

                foreach (var resposta in respostasPorPergunta.OrderBy(r => r.OrdenacaoResposta))
                {
                    var chave = Constantes.ListaChavesGraficos[chaveIndex++].ToString();

                    legendas.Add(new GraficoBarrasLegendaDto()
                    {
                        Chave = chave,
                        Valor = string.IsNullOrEmpty(resposta.Resposta) ? "Sem Preenchimento" : resposta.Resposta
                    });

                    grafico.EixosX.Add(new GraficoBarrasVerticalEixoXDto(resposta.Quantidade, chave));
                }

                var valorMaximoEixo = grafico.EixosX.Count > 0 ? grafico.EixosX.Max(a => int.Parse(a.Valor.ToString())) : 0;

                grafico.EixoYConfiguracao = new GraficoBarrasVerticalEixoYDto(350, "Quantidade Alunos", valorMaximoEixo.ArredondaParaProximaDezena(), 10);
                grafico.Legendas = legendas;

                relatorio.GraficosBarras.Add(grafico);
            }
        }

        private async Task<RelatorioSondagemComponentesPorTurmaCabecalhoDto> ObterCabecalho(ObterRelatorioSondagemComponentesPorTurmaQuery request)
        {
            var componenteCurricular = request.ComponenteCurricular.ShortName();
            var ue = await ObterUe(request.UeCodigo);
            var usuario = await usuarioRepository.ObterDados(request.UsuarioRF);

            List<RelatorioSondagemComponentesPorTurmaOrdemDto> ordens;
            List<RelatorioSondagemComponentesPorTurmaPerguntaDto> perguntas;

            string periodo;
            string proficiencia;

            if (request.AnoLetivo < 2022)
            {
                ordens = (await ObterOrdens(request.Ano, request.Proficiencia)).ToList();
                perguntas = await ObterPerguntas(request.Proficiencia, request.Ano);
                periodo = $"{request.Semestre}° Semestre";
                proficiencia = int.Parse(request.Ano) >= 7 ? string.Empty : request.Proficiencia.Name();
            }
            else
            {
                ordens = (await ObterOrdens(request)).ToList();
                perguntas = await ObterPerguntas(request);
                periodo = $"{request.Bimestre}° Bimestre";
                proficiencia = int.Parse(request.Ano) >= 4 ? string.Empty : request.Proficiencia.Name();
            }

            var dre = await ObterDre(request.DreCodigo);
            var turma = await mediator.Send(new ObterTurmaSondagemEolPorCodigoQuery(request.TurmaCodigo));

            return new RelatorioSondagemComponentesPorTurmaCabecalhoDto()
            {
                Ano = request.Ano,
                AnoLetivo = request.AnoLetivo,
                ComponenteCurricular = componenteCurricular,
                DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                Dre = dre.Abreviacao,
                Periodo = periodo,
                Proficiencia = proficiencia,
                Turma = turma.Nome,
                Ue = ue.NomeComTipoEscola,
                Rf = request.UsuarioRF,
                Usuario = usuario.Nome,
                Ordens = ordens,
                Perguntas = perguntas
            };
        }

        private async Task<Dre> ObterDre(string dreCodigo)
        {
            return await mediator.Send(new ObterDrePorCodigoQuery() { DreCodigo = dreCodigo });
        }

        private async Task<IEnumerable<RelatorioSondagemComponentesPorTurmaOrdemDto>> ObterOrdensProficiencia(ObterRelatorioSondagemComponentesPorTurmaQuery request)
        {
            var listaSondagem = await relatorioSondagemComponentePorTurmaRepository.ObterPerguntasRespostasProficiencia(request.DreCodigo, request.TurmaCodigo.ToString(),
                request.AnoLetivo, request.Bimestre, request.Proficiencia, int.Parse(request.Ano), request.ComponenteCurricular.Name());

            var retorno = new List<RelatorioSondagemComponentesPorTurmaOrdemDto>();

            foreach (var item in listaSondagem)
            {
                retorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                {
                    Id = item.PerguntaId,
                    Descricao = item.Pergunta
                });
            }

            return retorno;
        }

        private async Task<IEnumerable<RelatorioSondagemComponentesPorTurmaOrdemDto>> ObterOrdens(ObterRelatorioSondagemComponentesPorTurmaQuery request)
        {
            if (int.Parse(request.Ano) <= 3)
                return await ObterOrdensProficiencia(request);

            return new List<RelatorioSondagemComponentesPorTurmaOrdemDto>();
        }

        private async Task<IEnumerable<RelatorioSondagemComponentesPorTurmaOrdemDto>> ObterOrdens(string ano, ProficienciaSondagemEnum proficiencia)
        {
            return await mediator.Send(new ObterOrdensSondagemPorAnoProficienciaQuery(ano, proficiencia));
        }

        private async Task<Ue> ObterUe(string ueCodigo)
        {
            return await mediator.Send(new ObterUePorCodigoQuery(ueCodigo));
        }

        private async Task<List<RelatorioSondagemComponentesPorTurmaPerguntaDto>> ObterPerguntasAutoral(ObterRelatorioSondagemComponentesPorTurmaQuery request)
        {
            var periodoPorTipo = await mediator.Send(new ObterPeriodoPorTipoQuery(request.Bimestre, TipoPeriodoSondagem.Bimestre));
            var periodoId = periodoPorTipo?.Id;

            var listaSondagem = await relatorioSondagemComponentePorTurmaRepository.ObterPerguntasRespostas(request.DreCodigo, request.TurmaCodigo.ToString(),
                request.AnoLetivo, request.Bimestre, int.Parse(request.Ano), request.ComponenteCurricular.Name(), periodoId);

            var retorno = new List<RelatorioSondagemComponentesPorTurmaPerguntaDto>();

            foreach (var item in listaSondagem)
            {
                retorno.Add(new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                {
                    Id = item.PerguntaId,
                    Nome = item.Pergunta
                });
            }

            return retorno;
        }

        private async Task<List<RelatorioSondagemComponentesPorTurmaPerguntaDto>> ObterPerguntasProficiencia(ObterRelatorioSondagemComponentesPorTurmaQuery request)
        {
            var listaSondagem = await relatorioSondagemComponentePorTurmaRepository.ObterPerguntasRespostasProficiencia(request.DreCodigo, request.TurmaCodigo.ToString(),
                request.AnoLetivo, request.Bimestre, request.Proficiencia, int.Parse(request.Ano), request.ComponenteCurricular.Name());

            var retorno = new List<RelatorioSondagemComponentesPorTurmaPerguntaDto>();

            foreach (var item in listaSondagem)
            {
                retorno.Add(new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                {
                    Id = item.PerguntaId,
                    Nome = item.SubPergunta
                });
            }

            return retorno;
        }

        private async Task<List<RelatorioSondagemComponentesPorTurmaPerguntaDto>> ObterPerguntas(ObterRelatorioSondagemComponentesPorTurmaQuery request)
        {
            if (int.Parse(request.Ano) <= 3)
                return await ObterPerguntasProficiencia(request);

            return await ObterPerguntasAutoral(request);
        }

        private async Task<List<RelatorioSondagemComponentesPorTurmaPerguntaDto>> ObterPerguntas(ProficienciaSondagemEnum proficiencia, string ano)
        {
            if (int.Parse(ano) >= 7)
            {
                return await Task.FromResult(new List<RelatorioSondagemComponentesPorTurmaPerguntaDto>()
                {
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 1,
                        Nome = "Problema de lógica"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 2,
                        Nome = "Área e perímetro"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 3,
                        Nome = (ano == "8")?"Triângulos e quadriláteros":"Sólidos geométricos"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 4,
                        Nome = (ano == "9")?"Regularidade e generalização":"Relações entre grandezas e porcentagem"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 5,
                        Nome = (ano == "7")?"Média, moda e mediana":"Probabilidade"
                    },
                });
            }

            if (proficiencia == ProficienciaSondagemEnum.CampoAditivo || proficiencia == ProficienciaSondagemEnum.CampoMultiplicativo)
            {
                return await Task.FromResult(new List<RelatorioSondagemComponentesPorTurmaPerguntaDto>()
                {
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 1,
                        Nome = "Ideia"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 2,
                        Nome = "Resultado"
                    }
                });
            }

            if (proficiencia == ProficienciaSondagemEnum.Numeros)
            {
                return await Task.FromResult(new List<RelatorioSondagemComponentesPorTurmaPerguntaDto>()
                {
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 1,
                        Nome = "Familiares ou Frequentes"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 2,
                        Nome = "Opacos"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 3,
                        Nome = "Transparentes"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 4,
                        Nome = "Terminam em Zero"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 5,
                        Nome = "Algarismos Iguais"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 6,
                        Nome = "Processo de Generalização"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 7,
                        Nome = "Zero Intercalado"
                    },
                });
            }

            return await Task.FromResult(new List<RelatorioSondagemComponentesPorTurmaPerguntaDto>());
        }

        private async Task<RelatorioSondagemComponentesPorTurmaPlanilhaDto> ObterPlanilhaPerguntasRespostasAutoral(ObterRelatorioSondagemComponentesPorTurmaQuery request,
            List<RelatorioSondagemComponentesPorTurmaPerguntaDto> perguntas)
        {
            var periodoPorTipo = await mediator.Send(new ObterPeriodoPorTipoQuery(request.Bimestre, TipoPeriodoSondagem.Bimestre));
            var periodoId = periodoPorTipo?.Id;

            var listaSondagem = await relatorioSondagemComponentePorTurmaRepository.ObterPerguntasRespostas(request.DreCodigo, request.TurmaCodigo.ToString(),
                request.AnoLetivo, request.Bimestre, int.Parse(request.Ano), request.ComponenteCurricular.Name(), periodoId);

            List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto> linhasPlanilhaQueryDto = new List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>();

            foreach (var aluno in request.alunos.OrderBy(a => a.ObterNomeFinal()))
            {
                var listaRespostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>();

                foreach (var pergunta in perguntas)
                {
                    var resposta = listaSondagem.FirstOrDefault(c => c.AlunoEolCode == aluno.CodigoAluno.ToString() &&
                        c.PerguntaId == pergunta.Id);

                    if (resposta != null)
                    {
                        listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                        {
                            OrdemId = 0,
                            AnoLetivo = request.AnoLetivo,
                            PerguntaId = pergunta.Id,
                            Resposta = resposta.Resposta,
                            OrdenacaoResposta = resposta.OrdenacaoResposta
                        });
                    }
                    else
                    {
                        listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                        {
                            OrdemId = 0,
                            AnoLetivo = request.AnoLetivo,
                            PerguntaId = pergunta.Id,
                            Resposta = string.Empty,
                            OrdenacaoResposta = 0
                        });
                    }
                }

                linhasPlanilhaQueryDto.Add(new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto()
                {
                    Aluno = TransformarAlunoDto(aluno),
                    OrdensRespostas = listaRespostas
                });
            }

            return new RelatorioSondagemComponentesPorTurmaPlanilhaDto() { Linhas = linhasPlanilhaQueryDto };
        }

        private async Task<RelatorioSondagemComponentesPorTurmaPlanilhaDto> ObterPlanilhaAutoral(ObterRelatorioSondagemComponentesPorTurmaQuery request, List<RelatorioSondagemComponentesPorTurmaPerguntaDto> perguntas)
        {
            var periodoPorTipo = await mediator.Send(new ObterPeriodoPorTipoQuery(request.Semestre, TipoPeriodoSondagem.Semestre));
            var periodoId = periodoPorTipo?.Id;

            var listaSondagem = await relatorioSondagemComponentePorTurmaRepository.ObterPlanilhaLinhas(request.DreCodigo, request.TurmaCodigo.ToString(),
                request.AnoLetivo, request.Semestre, request.Proficiencia, int.Parse(request.Ano), periodoId);

            List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto> linhasPlanilhaQueryDto = new List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>();

            foreach (var aluno in request.alunos.OrderBy(a => a.ObterNomeFinal()))
            {
                var listaRespostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>();

                foreach (RelatorioSondagemComponentesPorTurmaPerguntaDto pergunta in perguntas)
                {
                    RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto resposta = listaSondagem.FirstOrDefault(r => r.AlunoEolCode == aluno.CodigoAluno.ToString() &&
                        r.PerguntaId == pergunta.Id);

                    if (resposta != null)
                    {
                        listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                        {
                            OrdemId = 0,
                            AnoLetivo = request.AnoLetivo,
                            PerguntaId = pergunta.Id,
                            Resposta = resposta.Resposta,
                            OrdenacaoResposta = resposta.OrdenacaoResposta
                        });
                    }
                    else
                    {
                        listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                        {
                            OrdemId = 0,
                            AnoLetivo = request.AnoLetivo,
                            PerguntaId = pergunta.Id,
                            Resposta = string.Empty,
                        });
                    }
                }

                linhasPlanilhaQueryDto.Add(new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto()
                {
                    Aluno = TransformarAlunoDto(aluno),
                    OrdensRespostas = listaRespostas
                });
            }

            return new RelatorioSondagemComponentesPorTurmaPlanilhaDto() { Linhas = linhasPlanilhaQueryDto };
        }

        private async Task<RelatorioSondagemComponentesPorTurmaPlanilhaDto> ObterPlanilhaPerguntasRespostasProficiencia(ObterRelatorioSondagemComponentesPorTurmaQuery request)
        {
            var listaSondagem = await relatorioSondagemComponentePorTurmaRepository.ObterPerguntasRespostasProficiencia(request.DreCodigo, request.TurmaCodigo.ToString(),
                request.AnoLetivo, request.Bimestre, request.Proficiencia, int.Parse(request.Ano), request.ComponenteCurricular.Name());

            var linhasPlanilhaQueryDto = new List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>();

            foreach (var aluno in request.alunos.OrderBy(a => a.ObterNomeFinal()))
            {
                var respostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>();

                foreach (var item in listaSondagem.Where(c => c.AlunoEolCode == aluno.CodigoAluno.ToString()))
                {
                    var ordemId = 0;

                    if (!request.Proficiencia.Equals(ProficienciaSondagemEnum.Numeros))
                        ordemId = item.OrdenacaoResposta;

                    respostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                    {
                        OrdemId = ordemId,
                        AnoLetivo = request.AnoLetivo,
                        PerguntaId = item.PerguntaId,
                        Resposta = item.Resposta,
                        OrdenacaoResposta = item.OrdenacaoResposta
                    });
                }

                linhasPlanilhaQueryDto.Add(new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto()
                {
                    Aluno = TransformarAlunoDto(aluno),
                    OrdensRespostas = respostas
                });
            }

            return new RelatorioSondagemComponentesPorTurmaPlanilhaDto() { Linhas = linhasPlanilhaQueryDto };
        }

        private async Task<RelatorioSondagemComponentesPorTurmaPlanilhaDto> ObterPlanilha(ObterRelatorioSondagemComponentesPorTurmaQuery request)
        {
            var listaSondagem = await relatorioSondagemComponentePorTurmaRepository.ObterPlanilhaLinhas(request.DreCodigo, request.TurmaCodigo.ToString(),
                request.AnoLetivo, request.Semestre, request.Proficiencia, int.Parse(request.Ano));

            List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto> linhasPlanilhaQueryDto = new List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>();

            foreach (var aluno in request.alunos.OrderBy(a => a.ObterNomeFinal()))
            {
                var respostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>();

                var respostaDoAluno = listaSondagem.FirstOrDefault(a => a.AlunoEolCode == aluno.CodigoAluno.ToString());

                respostas = await ObterOrdemRespostas(respostaDoAluno, request.Ano, request.Proficiencia);

                linhasPlanilhaQueryDto.Add(new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto()
                {
                    Aluno = TransformarAlunoDto(aluno),
                    OrdensRespostas = respostas
                });
            }

            return new RelatorioSondagemComponentesPorTurmaPlanilhaDto() { Linhas = linhasPlanilhaQueryDto };
        }

        private RelatorioSondagemComponentesPorTurmaAlunoDto TransformarAlunoDto(Aluno aluno)
        {
            return new RelatorioSondagemComponentesPorTurmaAlunoDto()
            {
                Codigo = aluno.CodigoAluno,
                DataSituacao = aluno.DataSituacao.ToString("dd/MM/yyyy"),
                Nome = aluno.ObterNomeParaRelatorioSondagem(),
                SituacaoMatricula = aluno.SituacaoMatricula
            };
        }

        private async Task<List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>> ObterOrdemRespostas(RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto linha, string ano, ProficienciaSondagemEnum proficiencia)
        {
            var listaRespostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>();

            switch (ano)
            {
                case "1":
                    ObterRespostasAno1(linha, listaRespostas, proficiencia);
                    break;
                case "2":
                    ObterRespostasAno2(linha, listaRespostas, proficiencia);
                    break;
                case "3":
                    ObterRespostasAno3(linha, listaRespostas, proficiencia);
                    break;
                case "4":
                    ObterRespostasAno4(linha, listaRespostas, proficiencia);
                    break;
                case "5":
                    ObterRespostasAno5(linha, listaRespostas, proficiencia);
                    break;
                case "6":
                    ObterRespostasAno6(linha, listaRespostas, proficiencia);
                    break;
            }

            return await Task.FromResult(listaRespostas);
        }

        private static void ObterRespostasNumeros(RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto linha, List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto> listaRespostas)
        {
            listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
            {
                OrdemId = 0,
                PerguntaId = 1,
                Resposta = linha?.Familiares,
            });

            listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
            {
                OrdemId = 0,
                PerguntaId = 2,
                Resposta = linha?.Opacos,
            });

            listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
            {
                OrdemId = 0,
                PerguntaId = 3,
                Resposta = linha?.Transparentes,
            });

            listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
            {
                OrdemId = 0,
                PerguntaId = 4,
                Resposta = linha?.TerminamZero,
            });

            listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
            {
                OrdemId = 0,
                PerguntaId = 5,
                Resposta = linha?.Algarismos,
            });

            listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
            {
                OrdemId = 0,
                PerguntaId = 6,
                Resposta = linha?.Processo,
            });

            listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
            {
                OrdemId = 0,
                PerguntaId = 7,
                Resposta = linha?.ZeroIntercalados,
            });
        }

        private static void ObterRespostasAno1(RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto linha, List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto> listaRespostas, ProficienciaSondagemEnum proficiencia)
        {
            if (proficiencia == ProficienciaSondagemEnum.Numeros)
            {
                ObterRespostasNumeros(linha, listaRespostas);
            }
            else if (proficiencia == ProficienciaSondagemEnum.CampoAditivo)
            {
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem1Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem1Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem2Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem2Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem3Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem3Resultado,
                });
            }
        }

        private static void ObterRespostasAno2(RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto linha, List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto> listaRespostas, ProficienciaSondagemEnum proficiencia)
        {
            if (proficiencia == ProficienciaSondagemEnum.Numeros)
            {
                ObterRespostasNumeros(linha, listaRespostas);
            }
            else if (proficiencia == ProficienciaSondagemEnum.CampoAditivo)
            {
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem1Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem1Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem2Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem2Resultado,
                });
            }
            else if (proficiencia == ProficienciaSondagemEnum.CampoMultiplicativo)
            {
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem3Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem3Resultado,
                });
            }
        }

        private static void ObterRespostasAno3(RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto linha, List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto> listaRespostas, ProficienciaSondagemEnum proficiencia)
        {
            if (proficiencia == ProficienciaSondagemEnum.Numeros)
            {
                ObterRespostasNumeros(linha, listaRespostas);
            }
            else if (proficiencia == ProficienciaSondagemEnum.CampoAditivo)
            {
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem1Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem1Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem2Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem2Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem3Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem3Resultado,
                });
            }
            else if (proficiencia == ProficienciaSondagemEnum.CampoMultiplicativo)
            {
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 4,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem4Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 4,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem4Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 5,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem5Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 5,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem5Resultado,
                });
            }
        }

        private static void ObterRespostasAno4(RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto linha, List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto> listaRespostas, ProficienciaSondagemEnum proficiencia)
        {
            if (proficiencia == ProficienciaSondagemEnum.CampoAditivo)
            {
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem1Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem1Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem2Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem2Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem3Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem3Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 4,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem4Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 4,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem4Resultado,
                });
            }
            else if (proficiencia == ProficienciaSondagemEnum.CampoMultiplicativo)
            {
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 5,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem5Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 5,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem5Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 6,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem6Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 6,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem6Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 7,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem7Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 7,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem7Resultado,
                });
            }
        }

        private static void ObterRespostasAno5(RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto linha, List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto> listaRespostas, ProficienciaSondagemEnum proficiencia)
        {
            if (proficiencia == ProficienciaSondagemEnum.CampoAditivo)
            {
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem1Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem1Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem2Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem2Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem3Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem3Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 4,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem4Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 4,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem4Resultado,
                });
            }
            else if (proficiencia == ProficienciaSondagemEnum.CampoMultiplicativo)
            {
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 5,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem5Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 5,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem5Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 6,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem6Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 6,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem6Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 7,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem7Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 7,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem7Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 8,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem8Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 8,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem8Resultado,
                });
            }
        }

        private static void ObterRespostasAno6(RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto linha, List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto> listaRespostas, ProficienciaSondagemEnum proficiencia)
        {
            if (proficiencia == ProficienciaSondagemEnum.CampoAditivo)
            {
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem1Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem1Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem2Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem2Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem3Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem3Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 4,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem4Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 4,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem4Resultado,
                });
            }
            else if (proficiencia == ProficienciaSondagemEnum.CampoMultiplicativo)
            {
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 5,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem5Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 5,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem5Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 6,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem6Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 6,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem6Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 7,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem7Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 7,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem7Resultado,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 8,
                    PerguntaId = 1,
                    Resposta = linha?.Ordem8Ideia,
                });

                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 8,
                    PerguntaId = 2,
                    Resposta = linha?.Ordem8Resultado,
                });

            }
        }
    }
}
