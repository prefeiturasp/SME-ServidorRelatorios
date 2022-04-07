using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.MVC.Models
{
    public class CriadorDeMockRelatorioPaginadoSondagemTurmaModel
    {
        public RelatorioSondagemComponentesPorTurmaRelatorioDto ObtenhaSondagemComponente()
        {
            return new RelatorioSondagemComponentesPorTurmaRelatorioDto()
            {
                Cabecalho = new RelatorioSondagemComponentesPorTurmaCabecalhoDto()
                {
                    Ano = 5.ToString(),
                    AnoLetivo = 2020,
                    ComponenteCurricular = "Matemática",
                    DataSolicitacao = DateTime.Now.ToString("dd/MM/YYYY"),
                    Dre = "DRE - BT",
                    Periodo = "1º Semestre",
                    Proficiencia = "Campo Aditivo",
                    Rf = "9879878",
                    Turma = "Todas",
                    Ue = "CEU EMEF BUTANTA",
                    Usuario = "Alice Gonçalves de Almeida Souza Nascimento da Silva Albuquerque",
                    Ordens = new List<RelatorioSondagemComponentesPorTurmaOrdemDto>()
                    {
                        new RelatorioSondagemComponentesPorTurmaOrdemDto()
                        {
                            Id = 1,
                            Descricao = "ORDEM 1 - COMPOSIÇÃO"
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemDto()
                        {
                            Id = 2,
                            Descricao = "ORDEM 2 - COMPOSIÇÃO"
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemDto()
                        {
                            Id = 3,
                            Descricao = "ORDEM 3 - COMPOSIÇÃO"
                        },
                    },
                    Perguntas = new List<RelatorioSondagemComponentesPorTurmaPerguntaDto>()
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
                    },
                },
                Planilha = new RelatorioSondagemComponentesPorTurmaPlanilhaDto()
                {
                    Linhas = ObtenhaLista()
                },
                GraficosBarras = new List<GraficoBarrasVerticalDto>() { ObtenhaGrafico() }
            };
        }

        private List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto> ObtenhaLista()
        {
            var linhas = new List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>();
            for (var i = 0; i < 40; i++)
            {
                linhas.Add(
                new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "ALEXIA FERNANDES LIMA ALEXIA FERNANDES LIMA ALEXIA FERNANDES LIMA - " + i,
                        SituacaoMatricula = SituacaoMatriculaAluno.Ativo.ToString(),
                    },
                    OrdensRespostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>()
                    {
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 1,
                            Resposta = "Errou",
                            PerguntaId = 1
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 1,
                            Resposta = "Acertou",
                            PerguntaId = 2
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 2,
                            Resposta = "Errou",
                            PerguntaId = 1
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 2,
                            Resposta = "Não resolveu",
                            PerguntaId = 2
                        },
                    },
                });
                linhas.Add(
                new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 4650630,
                        Nome = "MATHEUS GUILHERME NASCIMENTO DA SILVA (RECLASSIFICADO SAÍDA EM 11/04/2020) - " + i,
                        SituacaoMatricula = SituacaoMatriculaAluno.Desistente.ToString(),
                    },
                    OrdensRespostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>()
                    {
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 2,
                            Resposta = "Errou",
                            PerguntaId = 1
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 2,
                            Resposta = "Não resolveu",
                            PerguntaId = 2
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 3,
                            Resposta = "Errou",
                            PerguntaId = 1
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 3,
                            Resposta = "Acertou",
                            PerguntaId = 2
                        },
                    },
                });
                linhas.Add(
                new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "AMANDA ALBUQUERQUE - " + i,
                        SituacaoMatricula = SituacaoMatriculaAluno.NaoCompareceu.ToString(),
                    },
                    OrdensRespostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>()
                    {
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 1,
                            Resposta = "Errou",
                            PerguntaId = 1
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 1,
                            Resposta = "Acertou",
                            PerguntaId = 2
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 3,
                            Resposta = "Errou",
                            PerguntaId = 1
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 3,
                            Resposta = "Acertou",
                            PerguntaId = 2
                        },
                    },
                });
            }

            return linhas;
        }

        private GraficoBarrasVerticalDto ObtenhaGrafico()
        {
            var graficoBarras1 = new GraficoBarrasVerticalDto(600, "Teste - gráfico de matemática");

            graficoBarras1.Legendas = new List<GraficoBarrasLegendaDto>() {
                new GraficoBarrasLegendaDto()
                {
                    Chave="A",
                    Valor= "Não conseguiu ou não quis ler aaaa nmnnnn kkkk ssss"
                },
                new GraficoBarrasLegendaDto()
                {
                    Chave="B",
                    Valor= "Leu com muita dificuldade"
                },
                new GraficoBarrasLegendaDto()
                {
                    Chave="C",
                    Valor= "Leu com alguma fluencia"
                },
                new GraficoBarrasLegendaDto()
                {
                    Chave="D",
                    Valor= "Leu com fluencia"
                },
                new GraficoBarrasLegendaDto()
                {
                    Chave="E",
                    Valor= "Sem preenchimento"
                },
            };

            graficoBarras1.EixoYConfiguracao = new GraficoBarrasVerticalEixoYDto(350, "Quantidade Alunos", 24, 10);

            graficoBarras1.EixosX = new List<GraficoBarrasVerticalEixoXDto>()
            {
                new GraficoBarrasVerticalEixoXDto(2, "A"),
                new GraficoBarrasVerticalEixoXDto(2, "B"),
                new GraficoBarrasVerticalEixoXDto(2, "C"),
                new GraficoBarrasVerticalEixoXDto(1, "D"),
                new GraficoBarrasVerticalEixoXDto(24, "E"),
            };

            return graficoBarras1;
        }
    }
}
