using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class FechamentoAlunoRepository : IFechamentoAlunoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public FechamentoAlunoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<FechamentoAlunoAnotacaoConselho>> ObterAnotacoesTurmaAlunoBimestreAsync(string codigoAluno, long fechamentoTurmaId)
        {
            var query = FechamentoAlunoConsultas.AnotacoesAluno;
            var parametros = new { CodigoAluno = codigoAluno, FechamentoTurmaId = fechamentoTurmaId };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<FechamentoAlunoAnotacaoConselho>(query, parametros);
            }
        }
    }
}
