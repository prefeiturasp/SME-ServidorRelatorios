using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data.Repositories.Sgp
{
    public class RelatorioSondagemComponentePorTurmaRepository : IRelatorioSondagemComponentePorTurmaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public RelatorioSondagemComponentePorTurmaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }
        public async Task<RelatorioSondagemComponentesPorTurmaRelatorioDto> ObterRelatorio(int dreId, int turmaId, int ueId, int ano)
        {
            var parametros = new { dreId, ueId, turmaId, ano };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                RelatorioSondagemComponentesPorTurmaRelatorioDto relatorio = new RelatorioSondagemComponentesPorTurmaRelatorioDto()
                {
                    Cabecalho = ObterCabecalho(conexao, parametros),
                    Planilha = ObterPlanilha(conexao, parametros)
                };

                return await Task.FromResult(relatorio);
            }
        }

        private RelatorioSondagemComponentesPorTurmaCabecalhoDto ObterCabecalho(NpgsqlConnection conexao, object parametros)
        {
            var query = new StringBuilder();
            query.Append(@" aqui vai a query");

            return new RelatorioSondagemComponentesPorTurmaCabecalhoDto()
            {
                Ano = 2020,
                AnoLetivo = 2020,
                ComponenteCurricular = "Matemática",
                DataSolicitacao = DateTime.Now,
                Dre = "DRE - BT",
                Periodo = "1º Semestre",
                Proficiencia = "Campo Aditivo",
                Rf = "987987",
                Turma = "Todas",
                Ue = "CEU EMEF BUTANTA",
                Usuario = "master",
                Ordens = new List<RelatorioSondagemComponentesPorTurmaOrdemDto>()
                    {
                        new RelatorioSondagemComponentesPorTurmaOrdemDto()
                        {
                            Id = 1,
                            Nome = "ORDEM 1 - COMPOSIÇÃO"
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemDto()
                        {
                            Id = 2,
                            Nome = "ORDEM 2 - COMPOSIÇÃO"
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemDto()
                        {
                            Id = 3,
                            Nome = "ORDEM 3 - COMPOSIÇÃO"
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
            };
        }

        private RelatorioSondagemComponentesPorTurmaPlanilhaDto ObterPlanilha(NpgsqlConnection conexao, object parametros)
        {
            return new RelatorioSondagemComponentesPorTurmaPlanilhaDto()
            {
                Linhas = new List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>()
                    {
                        new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto() {
                            Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                            {
                                Codigo = 6197654,
                                Nome = "ALEXIA FERNANDES LIMA",
                                SituacaoMatricula = SituacaoMatriculaAluno.Ativo,
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
                                    Resposta = "Errou",
                                    PerguntaId = 2
                                },
                              new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 2,
                                    Resposta = "Acertou",
                                    PerguntaId = 1
                                },
                                 new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 2,
                                    Resposta = "Errou",
                                    PerguntaId = 2
                                },
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 3,
                                    PerguntaId = 1,
                                    Resposta = "Errou"
                                },
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 3,
                                    PerguntaId = 2,
                                    Resposta = "Acertou"
                                },
                            },
                        },
                        new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto() {
                            Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                            {
                                Codigo = 6195479,
                                Nome = "ALICE SILVA RIBEIRO",
                                SituacaoMatricula = SituacaoMatriculaAluno.Desistente,
                            },
                            OrdensRespostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>()
                            {
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 1,
                                    Resposta = "Acertou",
                                    PerguntaId = 1
                                },
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 1,
                                    Resposta = "Errou",
                                    PerguntaId = 2
                                },
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 2,
                                    Resposta = "Acertou",
                                    PerguntaId = 1
                                },
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 2,
                                    Resposta = "Errou",
                                    PerguntaId = 2
                                },
                            },
                        },
                        new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto() {
                            Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                            {
                                Codigo = 6197654,
                                Nome = "AMANDA ALBUQUERQUE",
                                SituacaoMatricula = SituacaoMatriculaAluno.NaoCompareceu,
                            },
                            OrdensRespostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>()
                            {
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 1,
                                    Resposta = "Acertou",
                                    PerguntaId = 1
                                },
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 1,
                                    Resposta = "Errou",
                                    PerguntaId = 2
                                },
                                 new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 2,
                                    Resposta = "Acertou",
                                    PerguntaId = 1
                                },
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 2,
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
                        },
                    }
            };
        }
    }
}
