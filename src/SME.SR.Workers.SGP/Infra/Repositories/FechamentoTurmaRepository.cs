using Dapper;
using Npgsql;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Commons.Config;
using SME.SR.Workers.SGP.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Infra
{
    public class FechamentoTurmaRepository : IFechamentoTurmaRepository
    {
        public async Task<FechamentoTurma> ObterTurmaPeriodoFechamentoPorId(long fechamentoTurmaId)
        {
            var query = FechamentoTurmaConsultas.FechamentoTurmaPeriodo;
            var parametros = new { FechamentoTurmaId = fechamentoTurmaId };

            using (var conexao = new NpgsqlConnection(ConnectionStrings.ConexaoSgp))
            {
                return (await conexao.QueryAsync<FechamentoTurma, Turma, PeriodoEscolar, FechamentoTurma>(query
                , (fechamentoTurma, turma, periodoEscolar) =>
                {
                    fechamentoTurma.Turma = turma;
                    fechamentoTurma.PeriodoEscolar = periodoEscolar;

                    return fechamentoTurma;
                }
                , parametros, splitOn: "TurmaId,CodigoTurma,Bimestre")).FirstOrDefault();
            }
        }
    }
}
