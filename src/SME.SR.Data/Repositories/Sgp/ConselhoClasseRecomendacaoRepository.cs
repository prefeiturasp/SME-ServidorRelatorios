using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ConselhoClasseRecomendacaoRepository : IConselhoClasseRecomendacaoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ConselhoClasseRecomendacaoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<ConselhoClasseRecomendacao>> ObterTodos()
        {
            var query = ConselhoClasseRecomendacaoConsultas.Listar;

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<ConselhoClasseRecomendacao>(query);
            }
        }
    }
}
