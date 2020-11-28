using Dapper;
using Npgsql;
using SME.SR.Data.Queries;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
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

        public async Task<long?> ObterCicloIdPorAnoModalidade(string ano, Modalidade modalidadeCodigo)
        {
            var query = CicloConsultas.ObterPorAnoModalidade;
            var parametros = new { Ano = ano, Modalidade = (int)modalidadeCodigo };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QuerySingleOrDefaultAsync<long?>(query, parametros);
            }
        }

        public async Task<IEnumerable<TipoCiclo>> ObterCiclosPorAnosModalidade(string[] anos, Modalidade modalidadeCodigo)
        {
            var query = @"select tc.id, tc.descricao, tca.ano from tipo_ciclo tc
                        inner join tipo_ciclo_ano tca on tc.id = tca.tipo_ciclo_id
                        where tca.ano = ANY(@anos) and tca.modalidade = @modalidade";

            var parametros = new { Anos = anos, Modalidade = (int)modalidadeCodigo };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<TipoCiclo>(query, parametros);
            }
        }

        public async Task<TipoCiclo> ObterPorId(long id)
        {
            var query = @"select tc.id, tc.descricao from tipo_ciclo tc
                        where tc.id = @Id";

            var parametros = new { Id = id };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<TipoCiclo>(query, parametros);
            }
        }

        public async Task<IEnumerable<TipoCiclo>> ObterPorUeId(long ueId)
        {


            var query = @"select distinct tc.descricao, tca.ano, tca.modalidade, tc.id from tipo_ciclo tc
                            inner join tipo_ciclo_ano tca on tc.id = tca.tipo_ciclo_id
                            inner join turma t on tca.ano = t.ano and tca.modalidade = t.modalidade_codigo
                            inner join ue u on t.ue_id  = u.id 
                                where u.id = @ueId";

            var parametros = new { ueId };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryAsync<TipoCiclo>(query, parametros);

        }

        public async Task<IEnumerable<TipoCiclo>> ObterCiclosPorModalidadeAsync(Modalidade modalidade)
        {


            var query = @"select tc.id, 
	                             tc.descricao,
	                             tca.ano,
	                             tca.modalidade from tipo_ciclo tc 
                          inner join tipo_ciclo_ano tca on tca.tipo_ciclo_id = tc.id 
                          where tca.modalidade = @modalidade
                          order by ano, tc.descricao";

            var parametros = new { modalidade };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryAsync<TipoCiclo>(query, parametros);

        }
    }
}
