using Dapper;
using Npgsql;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Commons.Config;
using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Infra
{
    public class FechamentoNotaRepository : IFechamentoNotaRepository
    {
        public async Task<IEnumerable<NotaConceitoBimestreComponente>> ObterNotasAlunoBimestre(long fechamentoTurmaId, string codigoAluno)
        {
            var query = FechamentoNotaConsultas.NotasAlunoBimestre;
            var parametros = new { FechamentoTurmaId = fechamentoTurmaId, CodigoAluno = codigoAluno};

            using (var conexao = new NpgsqlConnection(ConnectionStrings.ConexaoSgp))
            {
                return await conexao.QueryAsync<NotaConceitoBimestreComponente>(query, parametros);
            }
        }
    }
}
