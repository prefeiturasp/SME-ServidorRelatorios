using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ConselhoClasseNotaRepository : IConselhoClasseNotaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ConselhoClasseNotaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<NotaConceitoBimestreComponente>> ObterNotasAluno(long conselhoClasseId, string codigoAluno)
        {
            var query = ConselhoClasseNotaConsultas.NotasAlunoBimestre;
            var parametros = new { ConselhoClasseId = conselhoClasseId, CodigoAluno = codigoAluno };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<NotaConceitoBimestreComponente>(query, parametros);
            }
        }

        public async Task<IEnumerable<NotaConceitoBimestreComponente>> ObterNotasFinaisAlunoBimestre(string codigoTurma, string codigoAluno)
        {
            var query = ConselhoClasseNotaConsultas.NotasFinaisBimestre;
            var parametros = new { CodigoAluno = codigoAluno, CodigoTurma = codigoTurma };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<NotaConceitoBimestreComponente>(query, parametros);
            }
        }
    }
}
