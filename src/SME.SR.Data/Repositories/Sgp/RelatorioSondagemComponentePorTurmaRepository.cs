using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<RelatorioSondagemComponentesPorTurmaRelatorioDto> ObterRelatorio(int dreId, int turmaId, int ano, int semestre)
        {
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                RelatorioSondagemComponentesPorTurmaRelatorioDto relatorio = new RelatorioSondagemComponentesPorTurmaRelatorioDto()
                {
                    Cabecalho = ObterCabecalho(conexao, dreId, turmaId, ano, semestre),
                    Planilha = ObterPlanilha(conexao, dreId, turmaId, ano, semestre)
                };

                return await Task.FromResult(relatorio);
            }
        }

        private RelatorioSondagemComponentesPorTurmaCabecalhoDto ObterCabecalho(NpgsqlConnection conexao, int dreId, int turmaId, int ano, int semestre)
        {
            // TODO: Verificar como montar o restante de dados do cabeçalho
            return new RelatorioSondagemComponentesPorTurmaCabecalhoDto()
            {
                Ano = ano,
                AnoLetivo = ano,
                ComponenteCurricular = "Matemática",
                DataSolicitacao = DateTime.Now,
                Dre = new DreRepository(this.variaveisAmbiente).ObterPorCodigo(dreId.ToString()).Result.Abreviacao,
                Periodo = semestre.ToString(),
                Proficiencia = "Campo Aditivo",
                Turma = "Todas",
                Ue = "CEU EMEF BUTANTA",
                Rf = "987987",
                Usuario = "master",
                Ordens = ObterOrdens(conexao),
                Perguntas = ObterPerguntas()
            };
        }

        private List<RelatorioSondagemComponentesPorTurmaOrdemDto> ObterOrdens(NpgsqlConnection conexao)
        {
            return conexao.Query<RelatorioSondagemComponentesPorTurmaOrdemDto>(
                "select Id, Descricao from Ordem").ToList();
        }

        private List<RelatorioSondagemComponentesPorTurmaPerguntaDto> ObterPerguntas()
        {
            return new List<RelatorioSondagemComponentesPorTurmaPerguntaDto>()
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
                };
        }

        private RelatorioSondagemComponentesPorTurmaPlanilhaDto ObterPlanilha(NpgsqlConnection conexao, int dreId, int turmaId, int ano, int semestre)
        {
            string sql = @$"select
                        AlunoEolCode,
                        AlunoNome,
                        AnoLetivo,
                        AnoTurma,
                        Semestre,
                        Ordem1Ideia,
                        Ordem1Resultado,
                        Ordem2Ideia,
                        Ordem2Resultado,
                        Ordem3Ideia,
                        Ordem3Resultado,
                        Ordem4Ideia,
                        Ordem4Resultado
                        from MathPoolCAs
                        where
                        DreEolCode = { dreId }
                        and AnoLetivo = { ano }
                        and AnoTurma = { turmaId }
                        order by AlunoNome";

            List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto> linhasPlanilhaQueryDto = new List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>();
            foreach (var linha in conexao.Query<RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto>(sql).ToList())
            {
                linhasPlanilhaQueryDto.Add(new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto()
                {
                    Aluno = ObterAluno(linha.AlunoEolCode, linha.AlunoNome),
                    OrdensRespostas = ObterOrdemRespostas(linha)
                });
            }

            return new RelatorioSondagemComponentesPorTurmaPlanilhaDto() { Linhas = linhasPlanilhaQueryDto };
        }

        private RelatorioSondagemComponentesPorTurmaAlunoDto ObterAluno(string alunoEolCode, string alunoNome)
        {
            return new RelatorioSondagemComponentesPorTurmaAlunoDto()
            {
                Codigo = alunoEolCode,
                Nome = alunoNome,
                SituacaoMatricula = ObterSituacaoMatriculaAluno(alunoEolCode)
            };
        }

        private SituacaoMatriculaAluno ObterSituacaoMatriculaAluno(string alunoEolCode)
        {
            // TODO: Criar lógica
            return SituacaoMatriculaAluno.Ativo;
        }

        private List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto> ObterOrdemRespostas(RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto linha)
        {
            return new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>()
                {
                    new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                        OrdemId = 1,
                        PerguntaId = linha.Ordem1Ideia,
                        Resposta = linha.Ordem1Resultado,
                    },
                    new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                        OrdemId = 2,
                        PerguntaId = linha.Ordem2Ideia,
                        Resposta = linha.Ordem2Resultado,
                    },
                    new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                        OrdemId = 3,
                        PerguntaId = linha.Ordem3Ideia,
                        Resposta = linha.Ordem3Resultado,
                    },
                    new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                        OrdemId = 4,
                        PerguntaId = linha.Ordem4Ideia,
                        Resposta = linha.Ordem4Resultado,
                    },
                };
        }
    }
}
