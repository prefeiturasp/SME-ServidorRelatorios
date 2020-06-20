using Dapper;
using Npgsql;
using SME.SR.Data.Queries;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class CicloRepository : ICicloRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public CicloRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<long?> ObterCicloIdPorAnoModalidade(int ano, Modalidade modalidadeCodigo)
        {
            var query = CicloConsultas.ObterPorAnoModalidade;
            var parametros = new { Ano = ano.ToString(), Modalidade = (int)modalidadeCodigo };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QuerySingleOrDefaultAsync<long?>(query, parametros);
            }
        }
    }
}
