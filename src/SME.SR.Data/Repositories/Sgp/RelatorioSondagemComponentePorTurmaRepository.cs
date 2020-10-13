using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto>> ObterPlanilhaLinhas(string dreCodigo, string turmaCodigo, int ano, int semestre, ProficienciaSondagemEnum proficiencia)
        {
            string sql = String.Empty;

            sql = $"select \"AlunoEolCode\", \"AlunoNome\", \"AnoLetivo\", \"AnoTurma\",\"Semestre\",\"Ordem1Ideia\",\"Ordem1Resultado\",\"Ordem2Ideia\",\"Ordem2Resultado\",\"Ordem3Ideia\",\"Ordem3Resultado\",\"Ordem4Ideia\",\"Ordem4Resultado\" from \"MathPoolCAs\" where \"DreEolCode\" = @dreCodigo and \"AnoLetivo\" = @ano and \"TurmaEolCode\" = @turmaCodigo and \"Semestre\" = @semestre order by \"AlunoNome\"";

            if (proficiencia == ProficienciaSondagemEnum.CampoMultiplicativo)
                sql = $"select \"AlunoEolCode\", \"AlunoNome\", \"AnoLetivo\", \"AnoTurma\",\"Semestre\",\"Ordem3Ideia\",\"Ordem3Resultado\",\"Ordem4Ideia\",\"Ordem4Resultado\",\"Ordem5Ideia\",\"Ordem5Resultado\",\"Ordem6Ideia\",\"Ordem6Resultado\",\"Ordem7Ideia\",\"Ordem7Resultado\",\"Ordem8Ideia\",\"Ordem8Resultado\" from \"MathPoolCMs\" where \"DreEolCode\" = @dreCodigo and \"AnoLetivo\" = @ano and \"TurmaEolCode\" = @turmaCodigo and \"Semestre\" = @semestre order by \"AlunoNome\"";

            if (proficiencia == ProficienciaSondagemEnum.Numeros)
                sql = $"select \"AlunoEolCode\", \"AlunoNome\", \"AnoLetivo\", \"AnoTurma\",\"Semestre\",\"Familiares\",\"Opacos\",\"Transparentes\",\"TerminamZero\",\"Algarismos\",\"Processo\",\"ZeroIntercalados\" from \"MathPoolNumbers\" where \"DreEolCode\" = @dreCodigo and \"AnoLetivo\" = @ano and \"TurmaEolCode\" = @turmaCodigo and \"Semestre\" = @semestre order by \"AlunoNome\"";

            var parametros = new { dreCodigo, ano, turmaCodigo, semestre };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryAsync<RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto>(sql, parametros);

        }
    }
}
