using Dapper;
using Npgsql;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Commons.Config;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Infra
{
    public class ParametroSistemaRepository : IParametroSistemaRepository
    {
        public async Task<string> ObterValorPorTipo(TipoParametroSistema tipo)
        {
            var query = ParametroSistemaConsultas.ObterValor;
            var parametros = new { Tipo = tipo };

            using (var conexao = new NpgsqlConnection(ConnectionStrings.ConexaoSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<string>(query, parametros);
            }
        }
    }
}
