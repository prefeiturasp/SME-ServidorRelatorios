using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class FechamentoTurmaRepository : IFechamentoTurmaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public FechamentoTurmaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<FechamentoTurma> ObterTurmaPeriodoFechamentoPorId(long fechamentoTurmaId)
        {
            var query = FechamentoTurmaConsultas.FechamentoTurmaPeriodo;
            var parametros = new { FechamentoTurmaId = fechamentoTurmaId };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return (await conexao.QueryAsync<FechamentoTurma, Turma, PeriodoEscolar, FechamentoTurma>(query
                , (fechamentoTurma, turma, periodoEscolar) =>
                {
                    fechamentoTurma.Turma = turma;
                    fechamentoTurma.PeriodoEscolar = periodoEscolar;

                    return fechamentoTurma;
                }
                , parametros, splitOn: "TurmaId,Codigo,Bimestre")).FirstOrDefault();
            }
        }

        public async Task<IEnumerable<FechamentoTurma>> ObterFechamentosPorCodigosTurma(string[] codigosTurma)
        {
            var query = @"select f.id, t.turma_id TurmaId
                       from fechamento_turma f
                       inner join turma t on t.id = f.turma_id
                       where not f.excluido  
                       and t.turma_id = ANY(@turmasCodigo)";

            var parametros = new { TurmasCodigo = codigosTurma };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return (await conexao.QueryAsync<FechamentoTurma>(query, parametros));
            }
        }
    }
}
