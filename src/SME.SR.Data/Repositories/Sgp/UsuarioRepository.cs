using Dapper;
using Npgsql;
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

        public async Task<Usuario> ObterPorCodigoRF(string codigoRf)
        {
            var query = @"select rf_codigo CodigoRf, rf_codigo Login, nome from Usuario where rf_codigo = @codigoRf";
            var parametros = new { codigoRf };
            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryFirstOrDefaultAsync<Usuario>(query, parametros);
        }
    }
}
