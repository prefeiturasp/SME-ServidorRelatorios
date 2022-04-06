using Npgsql;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
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

        public async Task<IEnumerable<RelatorioSondagemComponentesPorTurmaOrdemDto>> ObterOrdensAsync()
        {
            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);
            return await conexao.QueryAsync<RelatorioSondagemComponentesPorTurmaOrdemDto>("select ROW_NUMBER () OVER (ORDER BY \"Id\") as Id, \"Descricao\" from public.\"Ordem\" ");
        }

        public async Task<IEnumerable<RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto>> ObterPlanilhaLinhas(string dreCodigo, string turmaCodigo, int anoLetivo,
            int semestre, ProficienciaSondagemEnum proficiencia, int anoTurma, string periodoId = "")
        {
            StringBuilder sql = new StringBuilder();

            if (anoTurma >= 7)
            {
                sql.AppendLine("select \"CodigoAluno\" AlunoEolCode, \"NomeAluno\" AlunoNome, \"AnoLetivo\", \"AnoTurma\", \"CodigoTurma\", pae.\"Ordenacao\" PerguntaId, p.\"Descricao\" Pergunta, r.\"Descricao\" Resposta, pr.\"Ordenacao\" OrdenacaoResposta ");
                sql.AppendLine(" from \"Sondagem\" s inner join \"SondagemAluno\" sa on sa.\"SondagemId\" = s.\"Id\" ");
                sql.AppendLine(" inner join \"SondagemAlunoRespostas\" sar on sar.\"SondagemAlunoId\" = sa.\"Id\"  ");
                sql.AppendLine(" inner join \"Pergunta\" p on sar.\"PerguntaId\" = p.\"Id\"  ");
                sql.AppendLine(" inner join \"PerguntaAnoEscolar\" pae on pae.\"PerguntaId\" = p.\"Id\" and pae.\"AnoEscolar\" = s.\"AnoTurma\"");
                sql.AppendLine(" inner join \"Resposta\" r on sar.\"RespostaId\" = r.\"Id\" ");
                sql.AppendLine(" inner join \"PerguntaResposta\" pr on pr.\"PerguntaId\" = p.\"Id\" and pr.\"RespostaId\" = r.\"Id\" ");
                sql.AppendLine(" where s.\"AnoLetivo\" = @anoLetivo and \"CodigoDre\" = @dreCodigo and \"AnoTurma\" = @anoTurma and \"CodigoTurma\" = @turmaCodigo and \"PeriodoId\" = @periodoId order by \"NomeAluno\" , pr.\"Ordenacao\"");
            }
            else
            {
                if (proficiencia == ProficienciaSondagemEnum.CampoMultiplicativo)
                {
                    sql.AppendLine("select \"AlunoEolCode\", \"AlunoNome\", \"AnoLetivo\", \"AnoTurma\",\"Semestre\",\"Ordem3Ideia\",\"Ordem3Resultado\",\"Ordem4Ideia\",\"Ordem4Resultado\",\"Ordem5Ideia\",\"Ordem5Resultado\",\"Ordem6Ideia\",\"Ordem6Resultado\",\"Ordem7Ideia\",\"Ordem7Resultado\",\"Ordem8Ideia\",\"Ordem8Resultado\" from \"MathPoolCMs\" where \"DreEolCode\" = @dreCodigo and \"AnoLetivo\" = @anoLetivo and \"TurmaEolCode\" = @turmaCodigo and \"Semestre\" = @semestre order by \"AlunoNome\"");
                }
                else if (proficiencia == ProficienciaSondagemEnum.Numeros)
                {
                    sql.AppendLine("select \"AlunoEolCode\", \"AlunoNome\", \"AnoLetivo\", \"AnoTurma\",\"Semestre\",\"Familiares\",\"Opacos\",\"Transparentes\",\"TerminamZero\",\"Algarismos\",\"Processo\",\"ZeroIntercalados\" from \"MathPoolNumbers\" where \"DreEolCode\" = @dreCodigo and \"AnoLetivo\" = @anoLetivo and \"TurmaEolCode\" = @turmaCodigo and \"Semestre\" = @semestre order by \"AlunoNome\"");
                }
                else
                {
                    sql.AppendLine("select \"AlunoEolCode\", \"AlunoNome\", \"AnoLetivo\", \"AnoTurma\",\"Semestre\",\"Ordem1Ideia\",\"Ordem1Resultado\",\"Ordem2Ideia\",\"Ordem2Resultado\",\"Ordem3Ideia\",\"Ordem3Resultado\",\"Ordem4Ideia\",\"Ordem4Resultado\" from \"MathPoolCAs\" where \"DreEolCode\" = @dreCodigo and \"AnoLetivo\" = @anoLetivo and \"TurmaEolCode\" = @turmaCodigo and \"Semestre\" = @semestre order by \"AlunoNome\"");
                }
            }

            var componenteCurricular = ComponenteCurricularSondagemEnum.Matematica.Name();

            var parametros = new { componenteCurricular, dreCodigo, anoLetivo, turmaCodigo, semestre, anoTurma, periodoId };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryAsync<RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto>(sql.ToString(), parametros);
        }
    }
}
