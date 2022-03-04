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
            var query = @"select Id, dre_id DreCodigo, Abreviacao, Nome from dre where dre_id = @dreCodigo";
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
    }
}
