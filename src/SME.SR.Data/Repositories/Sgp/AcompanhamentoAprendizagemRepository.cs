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
    public class AcompanhamentoAprendizagemRepository : IAcompanhamentoAprendizagemRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public AcompanhamentoAprendizagemRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<AcompanhamentoAprendizagemAlunoDto>> ObterAcompanhamentoAprendizagemPorTurmaESemestre(long turmaId, string alunoCodigo, int semestre)
        {
            var query = new StringBuilder(@"select at2.apanhado_geral as PercursoColetivoTurma,
                                               at2.semestre,
                                               aa.aluno_codigo as AlunoCodigo,
                                               aas.observacoes as Observacoes,
                                               aas.percurso_individual as PercursoIndividual
                                          from acompanhamento_turma at2
                                         inner join acompanhamento_aluno aa on aa.turma_id = at2.turma_id 
                                         inner join acompanhamento_aluno_semestre aas on aas.acompanhamento_aluno_id = aa.id and aas.semestre = at2.semestre 
                                        where at2.turma_id = @turmaId");

            if (!string.IsNullOrEmpty(alunoCodigo))
                query.AppendLine(" and aa.aluno_codigo = @alunoCodigo ");

            if (semestre > 0)
                query.AppendLine(" and aas.semestre = @semestre");

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<AcompanhamentoAprendizagemAlunoDto>(query.ToString(), new { turmaId, alunoCodigo, semestre });
            }
        }
    }
}
