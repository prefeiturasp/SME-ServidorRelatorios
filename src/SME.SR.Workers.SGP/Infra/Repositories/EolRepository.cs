using Dapper;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Commons.Config;
using SME.SR.Workers.SGP.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Infra
{
    public class EolRepository : IEolRepository
    {
        public async Task<List<Aluno>> ObterDadosAlunos()
        {
            try
            {
                using var conexao = new SqlConnection(ConnectionStrings.ConexaoEol);
                conexao.Open();
                IEnumerable<Aluno> list = await conexao.QueryAsync<Aluno>($"{AlunoConsultas.DadosAluno}", new { });
                conexao.Close();
                return list.ToList();
            } catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
