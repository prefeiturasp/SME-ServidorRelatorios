using Dapper;
using Npgsql;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Commons.Config;
using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Infra
{
    public class FechamentoAlunoRepository : IFechamentoAlunoRepository
    {
        public async Task<IEnumerable<FechamentoAlunoAnotacaoConselho>> ObterAnotacoesTurmaAlunoBimestreAsync(string codigoAluno, long fechamentoTurmaId)
        {
            var query = FechamentoAlunoConsultas.AnotacoesAluno;
            var parametros = new { CodigoAluno = codigoAluno, FechamentoTurmaId = fechamentoTurmaId };

            using (var conexao = new NpgsqlConnection(ConnectionStrings.ConexaoSgp))
            {
                return await conexao.QueryAsync<FechamentoAlunoAnotacaoConselho>(query, parametros);
            }
        }
    }
}
