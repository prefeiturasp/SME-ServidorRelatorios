using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Models;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            return await conexao.QueryAsync<ComponenteCurricular>(query, parametros);
        }

        public async Task<IEnumerable<ComponenteCurricularApiEol>> ListarApiEol()
        {
            var query = ComponenteCurricularConsultas.ListarApiEol;

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringApiEol);
            return await conexao.QueryAsync<ComponenteCurricularApiEol>(query);
        }

        public async Task<IEnumerable<ComponenteCurricularRegenciaApiEol>> ListarRegencia()
        {
            var query = ComponenteCurricularConsultas.ListarRegencia;

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringApiEol);
            return await conexao.QueryAsync<ComponenteCurricularRegenciaApiEol>(query);
        }

        public async Task<IEnumerable<Data.ComponenteCurricular>> ListarComponentesTerritorioSaber(string[] ids)
        {
            var query = ComponenteCurricularConsultas.BuscarTerritorioAgrupado(ids);

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            return await conexao.QueryAsync<ComponenteCurricular>(query);
        }

        public async Task<IEnumerable<ComponenteCurricularGrupoMatriz>> ListarGruposMatriz()
        {
            var query = ComponenteCurricularConsultas.ListarGruposMatriz;

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringApiEol);
            return await conexao.QueryAsync<ComponenteCurricularGrupoMatriz>(query);
        }

        public async Task<IEnumerable<ComponenteCurricularTerritorioSaber>> ObterComponentesTerritorioDosSaberes(string turmaCodigo, IEnumerable<long> componentesCurricularesId)
        {
       
            var query = ComponenteCurricularConsultas.BuscarTerritorioDoSaber;
            var parametros = new { CodigosComponentesCurriculares = componentesCurricularesId.ToArray(), CodigoTurma = turmaCodigo };

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryAsync<ComponenteCurricularTerritorioSaber>(query, parametros);
            }           
        }

        public async Task<IEnumerable<ComponenteCurricular>> ObterComponentesPorTurmaEProfessor(string login, string codigoTurma)
        {
            var query = ComponenteCurricularConsultas. BuscarPorTurmaEProfessor;
            var parametros = new { Login = login, CodigoTurma = codigoTurma };

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryAsync<ComponenteCurricular>(query, parametros);
            }
        }

        public async Task<IEnumerable<ComponenteCurricular>> ListarComponentes()
        {
            var query = ComponenteCurricularConsultas.Listar;

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryAsync<ComponenteCurricular>(query);
            }
        }
    }
}
