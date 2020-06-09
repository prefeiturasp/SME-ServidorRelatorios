using Dapper;
using Npgsql;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Commons.Config;
using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Infra.Repositories
{
    public class ConselhoClasseRecomendacaoRepository : IConselhoClasseRecomendacaoRepository
    {
        public async Task<IEnumerable<ConselhoClasseRecomendacao>> ObterTodos()
        {
            var query = ConselhoClasseRecomendacaoConsultas.Listar;

            using (var conexao = new NpgsqlConnection(ConnectionStrings.ConexaoSgp))
            {
                return await conexao.QueryAsync<ConselhoClasseRecomendacao>(query);
            }
        }
    }
}
