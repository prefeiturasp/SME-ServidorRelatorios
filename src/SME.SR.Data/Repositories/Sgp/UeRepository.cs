using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class UeRepository : IUeRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public UeRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<Ue> ObterPorCodigo(string UeCodigo)
        {
            var query = @"select Id, ue_id Codigo, Nome, tipo_escola TipoEscola from ue where ue_id = @ueCodigo";
            var parametros = new { UeCodigo };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<Ue>(query, parametros);
            }
        }
    }
}
