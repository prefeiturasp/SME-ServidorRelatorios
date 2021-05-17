using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class RegistroIndividualRepository : IRegistroIndividualRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public RegistroIndividualRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<RegistroIndividualRetornoDto>> ObterRegistrosIndividuaisPorTurmaEAluno(long turmaId, long[] alunosCodigo, DateTime dataInicio, DateTime dataFim)
        {
            var query = new StringBuilder(@"select turma_id as TurmaId,
	                                               aluno_codigo as AlunoCodigo,
	                                               registro,
	                                               data_registro as DataRegistro,
	                                               criado_por as CriadoPor,
	                                               criado_rf as CriadoRf 
                                              from registro_individual
                                             where turma_id = @turmaId
                                               and aluno_codigo = Any(@alunosCodigo)
                                               and data_registro::date between @dataInicio and @dataFim
                                               and not excluido ");           

            var parametros = new
            {
                turmaId,
                alunosCodigo,
                dataInicio,
                dataFim
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<RegistroIndividualRetornoDto>(query.ToString(), parametros);
            }
        }

        public async Task<IEnumerable<AcompanhamentoAprendizagemRegistroIndividualDto>> ObterRegistroIndividualPorTurmaEAluno(long turmaId, long? alunoCodigo)
        {
            var query = new StringBuilder(@"select ri.aluno_codigo as alunoCodigo, 
                                                   ri.turma_id as turmaId, 
                                                   ri.data_registro as dataRegistro, 
                                                   ri.registro 
                                              from registro_individual ri 
                                             where ri.turma_id = @turmaId                                               
                                               and not excluido ");

            if (alunoCodigo != null && alunoCodigo > 0)
                query.AppendLine("and ri.aluno_codigo = @alunoCodigo");

            var parametros = new { turmaId, alunoCodigo };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<AcompanhamentoAprendizagemRegistroIndividualDto>(query.ToString(), parametros);
            }
        }
    }
}
