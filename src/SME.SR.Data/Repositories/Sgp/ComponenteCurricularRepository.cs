using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ComponenteCurricularRepository : IComponenteCurricularRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ComponenteCurricularRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<ComponenteCurricular>> ObterComponentesPorTurma(string codigoTurma)
        {
            var query = ComponenteCurricularConsultas.BuscarPorTurma;
            var parametros = new { CodigoTurma = codigoTurma };

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryAsync<ComponenteCurricular>(query, parametros);
            }
        }

        public async Task<IEnumerable<ComponenteCurricularApiEol>> Listar()
        {
            var query = ComponenteCurricularConsultas.Listar;

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringApiEol))
            {
                return await conexao.QueryAsync<ComponenteCurricularApiEol>(query);
            }
        }

        public async Task<IEnumerable<ComponenteCurricularGrupoMatriz>> ListarGruposMatriz()
        {
            var query = ComponenteCurricularConsultas.ListarGruposMatriz;

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringApiEol))
            {
                return await conexao.QueryAsync<ComponenteCurricularGrupoMatriz>(query);
            }
        }
    }
}
