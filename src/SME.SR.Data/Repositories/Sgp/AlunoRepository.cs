using Dapper;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class AlunoRepository : IAlunoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public AlunoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<Aluno> ObterDados(string codigoTurma, string codigoAluno)
        {
            var query = AlunoConsultas.DadosAluno;
            var parametros = new { CodigoTurma = codigoTurma, CodigoAluno = codigoAluno };

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryFirstOrDefaultAsync<Aluno>(query, parametros);
            }
        }

        public async Task<IEnumerable<Aluno>> ObterPorCodigosAlunoETurma(string[] codigosTurma, string[] codigosAluno)
        {
            var query = AlunoConsultas.AlunosPorCodigoETurma;
            var parametros = new { CodigosTurma = codigosTurma, CodigosAluno = codigosAluno };

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryAsync<Aluno>(query, parametros);
            }
        }

        public async Task<IEnumerable<Aluno>> ObterPorCodigosTurma(string[] codigosTurma)
        {
            var query = AlunoConsultas.AlunosPorTurma;
            var parametros = new { CodigosTurma = codigosTurma };

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryAsync<Aluno>(query, parametros);
            }
        }
    }
}
