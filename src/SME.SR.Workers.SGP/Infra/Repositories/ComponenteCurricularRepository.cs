using Dapper;
using Npgsql;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Commons.Config;
using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Infra
{
    public class ComponenteCurricularRepository : IComponenteCurricularRepository
    {
        public async Task<IEnumerable<ComponenteCurricular>> ObterComponentesPorTurma(string codigoTurma)
        {
            var query = ComponenteCurricularConsultas.BuscarPorTurma;
            var parametros = new { CodigoTurma = codigoTurma };

            using (var conexao = new SqlConnection(ConnectionStrings.ConexaoEol))
            {
                return await conexao.QueryAsync<ComponenteCurricular>(query, parametros);
            }
        }

        public async Task<IEnumerable<ComponenteCurricularApiEol>> Listar()
        {
            var query = ComponenteCurricularConsultas.Listar;

            using (var conexao = new NpgsqlConnection(ConnectionStrings.ConexaoApiEol))
            {
                return await conexao.QueryAsync<ComponenteCurricularApiEol>(query);
            }
        }

        public async Task<IEnumerable<ComponenteCurricularGrupoMatriz>> ListarGruposMatriz()
        {
            var query = ComponenteCurricularConsultas.ListarGruposMatriz;

            using (var conexao = new NpgsqlConnection(ConnectionStrings.ConexaoApiEol))
            {
                return await conexao.QueryAsync<ComponenteCurricularGrupoMatriz>(query);
            }
        }
    }
}
