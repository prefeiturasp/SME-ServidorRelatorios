using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class RecuperacaoParalelaRepository : IRecuperacaoParalelaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public RecuperacaoParalelaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<RecuperacaoParalelaPeriodo> ObterPeriodoPorId(long id)
        {
            var query = @"select id, nome, descricao, bimestre_edicao
                         where not excluido and id = @Id";
            var parametros = new { Id = id } ;

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync(query, parametros);
            }
        }
    }
}
