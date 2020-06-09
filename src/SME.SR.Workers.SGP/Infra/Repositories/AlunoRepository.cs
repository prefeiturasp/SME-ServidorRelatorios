using Dapper;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Commons.Config;
using SME.SR.Workers.SGP.Models;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Infra
{
    public class AlunoRepository : IAlunoRepository
    {
        public async Task<Aluno> ObterDados(string codigoTurma, string codigoAluno)
        {
            var query = AlunoConsultas.DadosAluno;
            var parametros = new { CodigoTurma = codigoTurma, CodigoAluno = codigoAluno };

            using (var conexao = new SqlConnection(ConnectionStrings.ConexaoEol))
            {
                return await conexao.QueryFirstOrDefaultAsync<Aluno>(query, parametros);
            }
        }
    }
}
