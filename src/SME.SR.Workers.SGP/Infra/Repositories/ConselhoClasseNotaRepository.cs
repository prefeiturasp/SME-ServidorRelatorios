using Dapper;
using Npgsql;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Commons.Config;
using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Infra.Repositories
{
    public class ConselhoClasseNotaRepository : IConselhoClasseNotaRepository
    {
        public async Task<IEnumerable<NotaConceitoBimestreComponente>> ObterNotasAluno(long conselhoClasseId, string codigoAluno)
        {
            var query = ConselhoClasseNotaConsultas.NotasAlunoBimestre;
            var parametros = new { ConselhoClasseId = conselhoClasseId, CodigoAluno = codigoAluno };

            using (var conexao = new NpgsqlConnection(ConnectionStrings.ConexaoSgp))
            {
                return await conexao.QueryAsync<NotaConceitoBimestreComponente>(query, parametros);
            }
        }

        public async Task<IEnumerable<NotaConceitoBimestreComponente>> ObterNotasFinaisAlunoBimestre(string codigoTurma, string codigoAluno)
        {
            var query = ConselhoClasseNotaConsultas.NotasFinaisBimestre;
            var parametros = new { CodigoAluno = codigoAluno, CodigoTurma = codigoTurma };

            using (var conexao = new NpgsqlConnection(ConnectionStrings.ConexaoSgp))
            {
                return await conexao.QueryAsync<NotaConceitoBimestreComponente>(query, parametros);
            }
        }
    }
}
