using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class DreRepository : IDreRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public DreRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<Dre> ObterPorCodigo(string dreCodigo)
        {
            var query = @"select Id, dre_id Codigo, Abreviacao, Nome from dre where dre_id = @dreCodigo";
            var parametros = new { dreCodigo };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<Dre>(query, parametros);
            }

        }

        public async Task<Dre> ObterPorId(long dreId)
        {
            var query = @"select Id, dre_id Codigo, Abreviacao, Nome from dre where id = @dreId";
            var parametros = new { dreId };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<Dre>(query, parametros);
            }
        }

        public async Task<IEnumerable<Dre>> ObterTodas()
        {
            var query = @"select Id, dre_id Codigo, Abreviacao, Nome from dre";


            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryAsync<Dre>(query);

        }

        public async Task<DreUe> ObterDreUePorDreUeCodigo(string dreCodigo,string ueCodigo)
        {
            var query = @"
					select
						dre.id DreId,
						dre.dre_id DreCodigo,
						dre.abreviacao DreNome,
						ue.id UeId,
						ue.ue_id UeCodigo,
						concat(ue.ue_id, ' - ', tp.descricao, ' ', ue.nome) UeNome
					from  turma t
					inner join ue on ue.id = t.ue_id 
					inner join dre on ue.dre_id = dre.id 
					inner join tipo_escola tp on ue.tipo_escola = tp.cod_tipo_escola_eol 
				   where dre.dre_id = @dreCodigo AND ue.ue_id = @ueCodigo ";
            var parametros = new { dreCodigo,ueCodigo };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryFirstOrDefaultAsync<DreUe>(query, parametros);
        }
    }
}
