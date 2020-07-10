using Dapper;
using Npgsql;
using SME.SR.Data.Models;
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

        public async Task<IEnumerable<TipoCiclo>> ObterCiclosIdPorAnosModalidade(string[] anos, Modalidade modalidadeCodigo)
        {
            var query = @"select tc.id, tca.ano from tipo_ciclo tc
                        inner join tipo_ciclo_ano tca on tc.id = tca.tipo_ciclo_id
                        where tca.ano = ANY(@anos) and tca.modalidade = @modalidade";

            var parametros = new { Anos = anos, Modalidade = (int)modalidadeCodigo };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<TipoCiclo>(query, parametros);
            }
        }
    }
}
