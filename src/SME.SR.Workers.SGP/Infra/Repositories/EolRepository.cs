using Dapper;
using SME.SR.Workers.SGP.Commons.Config;
using SME.SR.Workers.SGP.Commons.Interfaces.Repositories;
using SME.SR.Workers.SGP.Infra.Queries;
using SME.SR.Workers.SGP.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Infra.Repositories
{
    public class EolRepository : IEolRepository
    {
        public async Task<List<DadosAluno>> ObterDadosAlunos()
        {
            try
            {
                using var conexao = new SqlConnection(ConnectionStrings.ConexaoEol);
                conexao.Open();
                IEnumerable<DadosAluno> list = await conexao.QueryAsync<DadosAluno>($"{AlunoConsultas.DadosAluno}", new { });
                conexao.Close();
                return list.ToList();
            } catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
