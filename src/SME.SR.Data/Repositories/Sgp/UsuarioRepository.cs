using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public UsuarioRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }
        public async Task<Usuario> ObterDados(string codigoRf)
        {
            string query = @"select rf_codigo as CodigoRf, login as Login, nome as Nome from usuario where rf_codigo = @codigoRf";

            var parametros = new { codigoRf };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<Usuario>(query, parametros);
            }
        }
    }
}
