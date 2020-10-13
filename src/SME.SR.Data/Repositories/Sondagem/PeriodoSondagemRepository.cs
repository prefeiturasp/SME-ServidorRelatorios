using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class PeriodoSondagemRepository : IPeriodoSondagemRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public PeriodoSondagemRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<DateTime> ObterPeriodoAberturaFimPorBimestreAnoLetivo(int bimestre, int anoLetivo)
        {
            var query = new StringBuilder("select \"DataFim\" ");
            query.AppendLine("from \"PeriodoDeAberturas\" ");
            query.AppendLine("where \"Ano\" = @anoLetivo");
            query.AppendLine("and \"Bimestre\" = @bimestre");

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryFirstOrDefaultAsync<DateTime>(query.ToString(), new { bimestre, anoLetivo = anoLetivo.ToString() });

        }

        public async Task<DateTime> ObterPeriodoFixoFimPorSemestreAnoLetivo(string semestreDescricao, int anoLetivo)
        {
            var query = new StringBuilder("select \"DataFim\" ");
            query.AppendLine("from \"PeriodoFixoAnual\" ");
            query.AppendLine("where \"Ano\" = @anoLetivo");
            query.AppendLine("and \"Descricao\" = @semestreDescricao");


            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryFirstOrDefaultAsync<DateTime>(query.ToString(), new { semestreDescricao, anoLetivo });
        }
    }
}
