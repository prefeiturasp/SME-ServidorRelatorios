using Dapper;
using Npgsql;
using SME.SR.Data.Queries;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Data.Repositories.Sgp
{
    public class NotaTipoRepository : INotaTipoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public NotaTipoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<string> ObterPorCicloIdDataAvalicacao(long? cicloId, DateTime dataReferencia)
        {
            var query = NotaTipoConsultas.ObterPorCicloIdDataAvaliacao;
            var parametros = new { CicloId = cicloId, DataReferencia = dataReferencia };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QuerySingleOrDefaultAsync<string>(query, parametros);
            };
        }
    }
}
