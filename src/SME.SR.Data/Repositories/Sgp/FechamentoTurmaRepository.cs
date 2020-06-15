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

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
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

        public async Task<IEnumerable<FechamentoTurma>> ObterTurmaPeriodoFechamentoPorId(string codigoTurma)
        {
            var query = FechamentoTurmaConsultas.FechamentosTurmaPorCodigoTurma;
            var parametros = new { TurmaCodigo = codigoTurma };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return (await conexao.QueryAsync<FechamentoTurma, Turma, PeriodoEscolar, FechamentoTurma>(query
                , (fechamentoTurma, turma, periodoEscolar) =>
                {
                    fechamentoTurma.Turma = turma;
                    fechamentoTurma.PeriodoEscolar = periodoEscolar;

                    return fechamentoTurma;
                }
                , parametros, splitOn: "id,CodigoTurma,Bimestre"));
            }
        }
    }
}
