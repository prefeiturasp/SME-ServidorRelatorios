using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ParametroSistemaRepository : IParametroSistemaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ParametroSistemaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<string> ObterValorPorTipo(TipoParametroSistema tipo)
        {
            var query = ParametroSistemaConsultas.ObterValor;
            var parametros = new { Tipo = tipo };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<string>(query, parametros);
            }
        }
    }
}
