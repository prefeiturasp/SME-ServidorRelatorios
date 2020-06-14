using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class FechamentoNotaRepository : IFechamentoNotaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public FechamentoNotaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<NotaConceitoBimestreComponente>> ObterNotasAlunoBimestre(long fechamentoTurmaId, string codigoAluno)
        {
            var query = FechamentoNotaConsultas.NotasAlunoBimestre;
            var parametros = new { FechamentoTurmaId = fechamentoTurmaId, CodigoAluno = codigoAluno};

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<NotaConceitoBimestreComponente>(query, parametros);
            }
        }
    }
}
