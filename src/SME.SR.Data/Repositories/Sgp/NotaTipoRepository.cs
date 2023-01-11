using Dapper;
using Npgsql;
using SME.SR.Data.Queries;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class NotaTipoRepository : INotaTipoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public NotaTipoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<TipoNotaCicloAno>> Listar()
        {
            var query = @"SELECT nccp.ciclo, tca.ano, tca.modalidade, CASE ntv.descricao 
                                WHEN 'Nota' THEN 'N'
                                ELSE 'C'
                            end TipoNota FROM notas_conceitos_ciclos_parametos nccp
                            inner join tipo_ciclo_ano tca on nccp.ciclo = tca.tipo_ciclo_id 
                            inner join tipo_ciclo tc on tca.tipo_ciclo_id = tc.id 
                            inner join notas_tipo_valor ntv on nccp.tipo_nota = ntv.id ";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<TipoNotaCicloAno>(query);
            };
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
            var query = @"select nccp.ciclo, ntv.descricao TipoNota from notas_tipo_valor ntv
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
