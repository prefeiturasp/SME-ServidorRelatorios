using Dapper;
using Npgsql;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public async Task<IEnumerable<RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto>> ObterPlanilhaLinhas(string dreCodigo, string turmaCodigo, int anoLetivo, int semestre, ProficienciaSondagemEnum proficiencia, int anoTurma)
        {
            string sql = String.Empty;

            sql = $"select \"AlunoEolCode\", \"AlunoNome\", \"AnoLetivo\", \"AnoTurma\",\"Semestre\",\"Ordem1Ideia\",\"Ordem1Resultado\",\"Ordem2Ideia\",\"Ordem2Resultado\",\"Ordem3Ideia\",\"Ordem3Resultado\",\"Ordem4Ideia\",\"Ordem4Resultado\" from \"MathPoolCAs\" where \"DreEolCode\" = @dreCodigo and \"AnoLetivo\" = @anoLetivo and \"TurmaEolCode\" = @turmaCodigo and \"Semestre\" = @semestre order by \"AlunoNome\"";

            if (proficiencia == ProficienciaSondagemEnum.CampoMultiplicativo)
                sql = $"select \"AlunoEolCode\", \"AlunoNome\", \"AnoLetivo\", \"AnoTurma\",\"Semestre\",\"Ordem3Ideia\",\"Ordem3Resultado\",\"Ordem4Ideia\",\"Ordem4Resultado\",\"Ordem5Ideia\",\"Ordem5Resultado\",\"Ordem6Ideia\",\"Ordem6Resultado\",\"Ordem7Ideia\",\"Ordem7Resultado\",\"Ordem8Ideia\",\"Ordem8Resultado\" from \"MathPoolCMs\" where \"DreEolCode\" = @dreCodigo and \"AnoLetivo\" = @anoLetivo and \"TurmaEolCode\" = @turmaCodigo and \"Semestre\" = @semestre order by \"AlunoNome\"";

            if (proficiencia == ProficienciaSondagemEnum.Numeros)
                sql = $"select \"AlunoEolCode\", \"AlunoNome\", \"AnoLetivo\", \"AnoTurma\",\"Semestre\",\"Familiares\",\"Opacos\",\"Transparentes\",\"TerminamZero\",\"Algarismos\",\"Processo\",\"ZeroIntercalados\" from \"MathPoolNumbers\" where \"DreEolCode\" = @dreCodigo and \"AnoLetivo\" = @anoLetivo and \"TurmaEolCode\" = @turmaCodigo and \"Semestre\" = @semestre order by \"AlunoNome\"";

            if (anoTurma >= 7)
            {
                sql = "select \"CodigoAluno\" AlunoEolCode, \"NomeAluno\" AlunoNome, \"AnoLetivo\", \"AnoTurma\", \"CodigoTurma\", pae.\"Ordenacao\" PerguntaId, p.\"Descricao\" Pergunta, r.\"Descricao\" Resposta";
                sql += " from \"SondagemAutoral\" sa inner join \"Pergunta\" p on sa.\"PerguntaId\" = p.\"Id\"";
                sql += " inner join \"ComponenteCurricular\" cc on p.\"ComponenteCurricularId\" = cc.\"Id\"";
                sql += " inner join \"PerguntaAnoEscolar\" pae on pae.\"PerguntaId\" = p.\"Id\" and pae.\"AnoEscolar\" = sa.\"AnoTurma\"";
                sql += " inner join \"Resposta\" r on sa.\"RespostaId\" = r.\"Id\"";
                sql += " where cc.\"Id\" = @componenteCurricular and sa.\"AnoLetivo\" = @anoLetivo and \"CodigoDre\" = @dreCodigo and \"AnoTurma\" = @anoTurma and \"CodigoTurma\" = @turmaCodigo order by \"NomeAluno\"";
            }

            var componenteCurricular = EnumExtensao.Name(ComponenteCurricularSondagemEnum.Matematica);

            var parametros = new { componenteCurricular, dreCodigo, anoLetivo, turmaCodigo, semestre, anoTurma };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryAsync<RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto>(sql, parametros);
        }
    }
}
