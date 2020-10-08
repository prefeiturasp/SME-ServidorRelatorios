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

        public async Task<IEnumerable<RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto>> ObterPlanilhaLinhas(string dreCodigo, string turmaCodigo, int ano, int semestre)
        {
            try
            {

            string sql = $"select \"AlunoEolCode\", \"AlunoNome\", \"AnoLetivo\", \"AnoTurma\",\"Semestre\",\"Ordem1Ideia\",\"Ordem1Resultado\",\"Ordem2Ideia\",\"Ordem2Resultado\",\"Ordem3Ideia\",\"Ordem3Resultado\",\"Ordem4Ideia\",\"Ordem4Resultado\" from \"MathPoolCAs\" where \"DreEolCode\" = @dreCodigo and \"AnoLetivo\" = @ano and \"TurmaEolCode\" = @turmaCodigo and \"Semestre\" = @semestre order by \"AlunoNome\"";
            
            var parametros = new { dreCodigo, ano, turmaCodigo, semestre };
           
            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);
            
            return await conexao.QueryAsync<RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto>(sql, parametros);

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
