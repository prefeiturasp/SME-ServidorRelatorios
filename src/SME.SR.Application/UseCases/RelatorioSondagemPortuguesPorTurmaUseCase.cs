﻿using MediatR;
using SME.SR.Application.Queries;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Extensions;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioSondagemPortuguesPorTurmaUseCase : IRelatorioSondagemPortuguesPorTurmaUseCase
    {
        private readonly IMediator mediator;

        public RelatorioSondagemPortuguesPorTurmaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<string> Executar(FiltroRelatorioSincronoDto request)
        {
            var filtros = request.ObterObjetoFiltro<RelatorioSondagemPortuguesPorTurmaFiltroDto>();

            if (filtros.ProficienciaId == ProficienciaSondagemEnum.Autoral && filtros.GrupoId == GrupoSondagemEnum.CapacidadeLeitura.Name())
                throw new NegocioException("Grupo fora do esperado.");
            
            var consideraNovaOpcaoRespostaSemPreenchimento = await mediator.Send(new UtilizarNovaOpcaoRespostaSemPreenchimentoQuery(filtros.Semestre,filtros.Bimestre,filtros.AnoLetivo));
            var periodoCompleto = await ObterDatasPeriodoFixoAnual(filtros.Bimestre, filtros.Semestre, filtros.AnoLetivo);

            var alunosDaTurma = await mediator.Send(new ObterAlunosPorTurmaDataSituacaoMatriculaQuery(int.Parse(filtros.TurmaCodigo), periodoCompleto.PeriodoFim, periodoCompleto.PeriodoInicio));
            if (alunosDaTurma == null || !alunosDaTurma.Any())
                throw new NegocioException("Não foi possível localizar os alunos da turma.");

            var relatorioPerguntas = await ObterPerguntas(filtros, consideraNovaOpcaoRespostaSemPreenchimento);

            var relatorio = new RelatorioSondagemPortuguesPorTurmaRelatorioDto()
            {
                Cabecalho = await ObterCabecalho(filtros, relatorioPerguntas),
                Planilha = new RelatorioSondagemPortuguesPorTurmaPlanilhaDto()
                {
                    Linhas = await ObterLinhas(filtros, alunosDaTurma, relatorioPerguntas)
                }
            };

            if (relatorio == null)
                throw new NegocioException("Não foi possível localizar dados com os filtros informados.");

            relatorio.GraficosBarras = new List<GraficoBarrasVerticalDto>();

            if (filtros.ProficienciaId == ProficienciaSondagemEnum.Leitura || filtros.ProficienciaId == ProficienciaSondagemEnum.Escrita)
            {
                var tipoRelatorio = filtros.ProficienciaId == ProficienciaSondagemEnum.Leitura ? "Leitura" : "Escrita";
                GerarGraficoLeituraEscrita(relatorio, tipoRelatorio, consideraNovaOpcaoRespostaSemPreenchimento);
            }

            if (filtros.ProficienciaId != ProficienciaSondagemEnum.Autoral ||
                (filtros.GrupoId != GrupoSondagemEnum.LeituraVozAlta.Name()
                 && filtros.GrupoId != GrupoSondagemEnum.ProducaoTexto.Name()))
                return await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioSondagemPortuguesPorTurma",
                    relatorio, Guid.NewGuid(), envioPorRabbit: false));
            {
                var tipoRelatorio = filtros.GrupoId == GrupoSondagemEnum.LeituraVozAlta.Name() ? "IAD - Leitura em voz alta" : "IAD - Produção de texto";
                GerarGraficoLeituraEmVozAltaProducaoTexto(relatorio, tipoRelatorio, consideraNovaOpcaoRespostaSemPreenchimento);
            }

            return await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioSondagemPortuguesPorTurma", relatorio, Guid.NewGuid(), envioPorRabbit: false));
        }

        private async Task<PeriodoCompletoSondagemDto> ObterDatasPeriodoFixoAnual(int bimestre, int semestre, int anoLetivo)
        {
            if (bimestre != 0)
                return await mediator.Send(new ObterDatasPeriodoSondagemPorBimestreAnoLetivoQuery(bimestre, anoLetivo));
            return await mediator.Send(new ObterDatasPeriodoSondagemPorSemestreAnoLetivoQuery(semestre, anoLetivo));
        }

        private static void GerarGraficoLeituraEscrita(RelatorioSondagemPortuguesPorTurmaRelatorioDto relatorio, string tipoRelatorio, bool consideraNovaOpcaoRespostaSemPreenchimento)
        {
            var grafico = new GraficoBarrasVerticalDto(800, $"Língua Portuguesa - {tipoRelatorio}");
            var chaveIndex = 0;
            var legendas = new List<GraficoBarrasLegendaDto>();
            var respostasAgrupadas = new List<RepostaTotalDto>();
            var totalSemPreenchimento = 0;

            foreach (var resposta in relatorio.Planilha.Linhas.Select(aluno => aluno.Respostas[0].Resposta))
            {
                if (!string.IsNullOrEmpty(resposta))
                {
                    var respostaAgrupada = respostasAgrupadas.FirstOrDefault(r => r.Resposta == resposta);
                    if (respostaAgrupada != null)
                    {
                        respostaAgrupada.Quantidade++;
                    }
                    else
                    {
                        respostasAgrupadas.Add(new RepostaTotalDto()
                        {
                            Resposta = resposta,
                            Quantidade = 1,
                        });
                    }
                }
                else
                {
                    totalSemPreenchimento++;
                }
            }

            respostasAgrupadas = respostasAgrupadas.OrderBy(r => r.Resposta).ToList();

            if (!consideraNovaOpcaoRespostaSemPreenchimento)
            {
                if (totalSemPreenchimento > 0)
                {
                    respostasAgrupadas.Add(new RepostaTotalDto()
                    {
                        Resposta= "Sem Preenchimento",
                        Quantidade = totalSemPreenchimento
                    });
                }
            }

            foreach (var resposta in respostasAgrupadas)
            {
                var chave = Constantes.ListaChavesGraficos[chaveIndex++].ToString();
                legendas.Add(new GraficoBarrasLegendaDto()
                {
                    Chave = chave,
                    Valor = resposta.Resposta
                });
                grafico.EixosX.Add(new GraficoBarrasVerticalEixoXDto(resposta.Quantidade, chave));
            }
            var valorMaximoEixo = grafico.EixosX.Any() ? grafico.EixosX.Max(a => int.Parse(a.Valor.ToString())) : 0;
            grafico.Legendas = legendas;
            grafico.EixoYConfiguracao = new GraficoBarrasVerticalEixoYDto(350, "Quantidade Alunos", valorMaximoEixo.ArredondaParaProximaDezena(), 10);
            relatorio.GraficosBarras.Add(grafico);
        }

        private static void GerarGraficoLeituraEmVozAltaProducaoTexto(RelatorioSondagemPortuguesPorTurmaRelatorioDto relatorio, string tipoRelatorio,bool consideraNovaOpcaoRespostaSemPreenchimento)
        {
            var grafico = new GraficoBarrasVerticalDto(800, $"Língua Portuguesa - {tipoRelatorio}");
            int chaveIndex = 0;
            var legendas = new List<GraficoBarrasLegendaDto>();

            foreach (var pergunta in relatorio.Cabecalho.Perguntas)
            {
                var chave = Constantes.ListaChavesGraficos[chaveIndex].ToString();
                legendas.Add(new GraficoBarrasLegendaDto()
                {
                    Chave = chave,
                    Valor = pergunta.Nome
                });
                grafico.EixosX.Add(new GraficoBarrasVerticalEixoXDto(0, chave));
                chaveIndex++;
            }

            var chaveLegendaSemPreenchimento = Constantes.ListaChavesGraficos[chaveIndex].ToString();

            if (!consideraNovaOpcaoRespostaSemPreenchimento)
            {
                legendas.Add(new GraficoBarrasLegendaDto()
                {
                    Chave = chaveLegendaSemPreenchimento,
                    Valor = "Sem preenchimento"
                });
                grafico.EixosX.Add(new GraficoBarrasVerticalEixoXDto(0, chaveLegendaSemPreenchimento));
            }
            

            foreach (var aluno in relatorio.Planilha.Linhas)
            {
                var totalRespostas = 0;
                foreach (var valorEixoX in from pergunta in relatorio.Cabecalho.Perguntas let respostaAluno = aluno.Respostas.FirstOrDefault(r => r.PerguntaId == pergunta.Id && !string.IsNullOrEmpty(r.Resposta)) where respostaAluno != null select legendas.FirstOrDefault(l => l.Valor == pergunta.Nome) into legenda select grafico.EixosX.FirstOrDefault(e => e.Titulo == legenda.Chave))
                {
                    valorEixoX.Valor++;
                    totalRespostas++;
                }

                if (consideraNovaOpcaoRespostaSemPreenchimento) continue;
                {
                    if (totalRespostas != 0) continue;
                    var valorEixoX = grafico.EixosX.FirstOrDefault(e => e.Titulo == chaveLegendaSemPreenchimento);
                    valorEixoX.Valor++;
                }
            }
            var valorMaximoEixo = grafico.EixosX.Max(a => int.Parse(a.Valor.ToString()));
            grafico.Legendas = legendas;
            grafico.EixoYConfiguracao = new GraficoBarrasVerticalEixoYDto(320, "Quantidade Alunos", valorMaximoEixo.ArredondaParaProximaDezena(), 10);
            relatorio.GraficosBarras.Add(grafico);
        }

        private async Task<RelatorioSondagemPortuguesPorTurmaCabecalhoDto> ObterCabecalho(RelatorioSondagemPortuguesPorTurmaFiltroDto filtros, List<RelatorioSondagemPortuguesPorTurmaPerguntaDto> perguntas)
        {
            var ue = await mediator.Send(new ObterUePorCodigoQuery(filtros.UeCodigo));
            var usuario = await mediator.Send(new ObterUsuarioPorCodigoRfQuery() { UsuarioRf = filtros.UsuarioRF });
            var dre = await mediator.Send(new ObterDrePorCodigoQuery() { DreCodigo = filtros.DreCodigo });
            var turma = await mediator.Send(new ObterTurmaSondagemEolPorCodigoQuery(int.Parse(filtros.TurmaCodigo)));

            var proficiencia = !string.IsNullOrEmpty(filtros.GrupoId) ? filtros.GrupoId : filtros.ProficienciaId.ToString();
            if (proficiencia == GrupoSondagemEnum.CapacidadeLeitura.Name())
            {
                proficiencia = GrupoSondagemEnum.CapacidadeLeitura.ShortName();
            }
            else if (proficiencia == GrupoSondagemEnum.LeituraVozAlta.Name())
            {
                proficiencia = GrupoSondagemEnum.LeituraVozAlta.ShortName();
            }
            else if (proficiencia == GrupoSondagemEnum.ProducaoTexto.Name())
            {
                proficiencia = GrupoSondagemEnum.ProducaoTexto.ShortName();
            }

            return await Task.FromResult(new RelatorioSondagemPortuguesPorTurmaCabecalhoDto()
            {
                DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                Dre = dre.Abreviacao,
                Periodo = $"{ Math.Max(filtros.Bimestre, filtros.Semestre) }° {(filtros.Bimestre != 0 ? "Bimestre" : "Semestre")}",
                Rf = filtros.UsuarioRF,
                Turma = turma.Nome,
                Ue = ue.NomeComTipoEscola,
                Usuario = usuario.Nome,
                AnoLetivo = filtros.AnoLetivo,
                Perguntas = perguntas,
                AnoTurma = filtros.Ano,
                ComponenteCurricular = ComponenteCurricularSondagemEnum.Portugues.ShortName(),
                Proficiencia = proficiencia
            });
        }

        private async Task<List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>> ObterPerguntas(RelatorioSondagemPortuguesPorTurmaFiltroDto filtros, bool consideraNovaOpcaoRespostaSemPreenchimento)
        {
            switch (filtros.ProficienciaId)
            {
                case ProficienciaSondagemEnum.Leitura:
                case ProficienciaSondagemEnum.Escrita:
                    return await Task.FromResult(new List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>()
                        {
                            new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                            {
                                Id = "1",
                                Nome = "Proficiência"
                            },
                        });
                case ProficienciaSondagemEnum.Autoral:
                    if (filtros.GrupoId == GrupoSondagemEnum.LeituraVozAlta.Name())
                    {
                        var perguntas = new List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>()
                                {
                                new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                                {
                                    Id = "0bf845cc-29dc-45ec-8bf2-8981cef616df",
                                    Nome = "Não conseguiu ou não quis ler"
                                },
                                new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                                {
                                    Id = "49c26883-e717-44aa-9aab-1bd8aa870916",
                                    Nome = "Leu com muita dificuldade"
                                },
                                new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                                {
                                    Id = "0b38221a-9d50-4cdf-abbd-a9ac092dbe70",
                                    Nome = "Leu com alguma fluência"
                                },
                                new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                                {
                                    Id = "18d148be-d83c-4f24-9d03-dc003a05b9e4",
                                    Nome = "Leu com fluência"
                                },
                            };

                        AdicionarPerguntaSemPreenchimento(perguntas, consideraNovaOpcaoRespostaSemPreenchimento);

                        return await Task.FromResult(perguntas);
                    }
                    if (filtros.GrupoId == GrupoSondagemEnum.ProducaoTexto.Name())
                    {
                        var perguntas = new List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>()
                                        {
                                            new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                                            {
                                                Id = "3173bff2-a148-4634-b029-b50c949ae2d6",
                                                Nome = "Não produziu/entregou em branco"
                                            },
                                            new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                                            {
                                                Id = "98940cdb-d229-4282-a2e1-60e4a17dab64",
                                                Nome = "Não apresentou dificuldades"
                                            },
                                            new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                                            {
                                                Id = "cfec69be-16fb-453d-8c47-fd5ebc4161ef",
                                                Nome = "Escrita não alfabética"
                                            },
                                            new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                                            {
                                                Id = "ef0e79cd-dc31-4272-ad04-68f79a3a135d",
                                                Nome = "Dificuldades com aspectos semânticos"
                                            },
                                            new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                                            {
                                                Id = "f4aae748-bfd8-482e-aee0-07a1cdad71ff",
                                                Nome = "Dificuldades com aspectos textuais"
                                            },
                                            new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                                            {
                                                Id = "67a791d2-089d-40ee-8ddf-c64454ee5c54",
                                                Nome = "Dificuldades com aspectos ortográficos e notacionais"
                                            }
                                        };

                        AdicionarPerguntaSemPreenchimento(perguntas, consideraNovaOpcaoRespostaSemPreenchimento);

                        return await Task.FromResult(perguntas);
                    }
                    else return await Task.FromResult(new List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>());
                default:
                    return await Task.FromResult(new List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>());
            }
        }

        private void AdicionarPerguntaSemPreenchimento(List<RelatorioSondagemPortuguesPorTurmaPerguntaDto> perguntas, bool consideraSemPreenchimento)
        {
            if (consideraSemPreenchimento)
                perguntas.Add(new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                {
                    Id = "0882766a-9375-4e0a-bd39-8d96d75f7a22",
                    Nome = "Sem preenchimento"
                });
        }

        private async Task<List<RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto>> ObterLinhas(
                                                    RelatorioSondagemPortuguesPorTurmaFiltroDto filtros, 
                                                    IEnumerable<Aluno> alunos,
                                                    List<RelatorioSondagemPortuguesPorTurmaPerguntaDto> perguntas)
        {
            var grupo = filtros.GrupoId == GrupoSondagemEnum.LeituraVozAlta.Name() ? GrupoSondagemEnum.LeituraVozAlta : GrupoSondagemEnum.ProducaoTexto;

            var linhasSondagem = await mediator.Send(new ObterRelatorioSondagemPortuguesPorTurmaQuery()
            {
                DreCodigo = filtros.DreCodigo,
                UeCodigo = filtros.UeCodigo,
                TurmaCodigo = filtros.TurmaCodigo,
                AnoLetivo = filtros.AnoLetivo,
                AnoTurma = filtros.Ano,
                Bimestre = filtros.Bimestre,
                Proficiencia = filtros.ProficienciaId,
                Grupo = grupo,
                Semestre = filtros.Semestre,
            });

            var linhasPlanilha = new List<RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto>();
            foreach (var aluno in alunos.OrderBy(a => a.NomeAluno).ToList())
            {
                var alunoDto = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                {
                    Codigo = aluno.CodigoAluno,
                    Nome = aluno.ObterNomeParaRelatorioSondagem(),
                    DataSituacao = aluno.DataSituacao.ToString("dd/MM/yyyy"),
                    SituacaoMatricula = aluno.SituacaoMatricula
                };

                var respostasDto = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>();

                foreach (var pergunta in perguntas)
                {
                    var resposta = linhasSondagem.FirstOrDefault(a => a.AlunoEolCode == aluno.CodigoAluno.ToString() && a.PerguntaId == pergunta.Id);
                    if (resposta != null)
                    {
                        resposta.Resposta = resposta.Resposta switch
                        {
                            "Nivel1" => "Nível 1",
                            "Nivel2" => "Nível 2",
                            "Nivel3" => "Nível 3",
                            "Nivel4" => "Nível 4",
                            "SemPreenchimento" => "Sem Preenchimento",
                            _ => resposta.Resposta
                        };
                    }
                    respostasDto.Add(new RelatorioSondagemPortuguesPorTurmaRespostaDto()
                    {
                        PerguntaId = pergunta.Id,
                        Resposta = resposta?.Resposta
                    });
                }


                linhasPlanilha.Add(new RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto()
                {
                    Aluno = alunoDto,
                    Respostas = respostasDto
                });
            }
            return await Task.FromResult(linhasPlanilha);
        }
    }
}
