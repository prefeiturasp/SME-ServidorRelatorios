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

        public async Task<IEnumerable<AlunoHistoricoEscolar>> ObterDadosAlunoHistoricoEscolar(string[] codigosAlunos)
        {
            var query = AlunoConsultas.DadosAlunosHistoricoEscolar;
            var parametros = new { CodigosAluno = codigosAlunos };

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryAsync<AlunoHistoricoEscolar>(query, parametros);
            }
        }
    }
}
