using Dapper;
using Npgsql;
using SME.SR.Data.Queries;
using SME.SR.Infra;
using System;
using System.Collections;
using System.Collections.Generic;
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

        public async Task<IEnumerable<TipoNotaCicloAno>> ObterPorCiclosIdDataAvalicacao(long[] ciclosId, DateTime dataReferencia)
        {
            var query = @"select nccp.ciclo, ntv.descricao from notas_tipo_valor ntv
                        inner join notas_conceitos_ciclos_parametos nccp
                        on nccp.tipo_nota = ntv.id
                        where nccp.ciclo = ANY(@ciclosId) and @dataReferencia >= nccp.inicio_vigencia
                        and (nccp.ativo = true or @dataReferencia <= nccp.fim_vigencia)
                        order by nccp.id asc";

            var parametros = new { CiclosId = ciclosId, DataReferencia = dataReferencia };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<TipoNotaCicloAno>(query, parametros);
            };
        }
    }
}
