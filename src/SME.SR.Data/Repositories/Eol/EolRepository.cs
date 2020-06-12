using Dapper;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class EolRepository : IEolRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public EolRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<List<Aluno>> ObterDadosAlunos()
        {
            try
            {
                using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
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
