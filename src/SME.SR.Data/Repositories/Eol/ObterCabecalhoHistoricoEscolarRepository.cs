using Dapper;
using SME.SR.Infra;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ObterCabecalhoHistoricoEscolarRepository : IObterCabecalhoHistoricoEscolarRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ObterCabecalhoHistoricoEscolarRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<CabecalhoDto> ObterCabecalhoHistoricoEscolar(int anoLetivo, string dreCodigo, string ueCodigo)
        {
            var query = @"SELECT * FROM ... ";

            using (var conn = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                conn.Open();
                var cabecalho = await conn.QueryFirstOrDefaultAsync<CabecalhoDto>(query, new { anoLetivo, dreCodigo, ueCodigo });
                conn.Close();
                return cabecalho;
            }
        }
    }
}
