using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class DreRepository : IDreRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public DreRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<Dre> ObterPorCodigo(string dreCodigo)
        {
            var query = @"select Id, dre_id Codigo, Abreviacao, Nome from dre where dre_id = @dreCodigo";
            var parametros = new { dreCodigo };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<Dre>(query, parametros);
            }
        }
    }
}
