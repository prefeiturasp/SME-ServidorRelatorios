using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
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
            this.variaveisAmbiente = variaveisAmbiente ?? throw new System.ArgumentNullException(nameof(variaveisAmbiente));
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

            if(alunoCodigo != null && alunoCodigo > 0)
                query.AppendLine("and ri.aluno_codigo = @alunoCodigo");

            var parametros = new { turmaId, alunoCodigo };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<AcompanhamentoAprendizagemRegistroIndividualDto>(query.ToString(), parametros);
            }
        }
    }
}
