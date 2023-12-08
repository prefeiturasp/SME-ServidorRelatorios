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
                                        where at2.turma_id = @turmaId and
                                              not at2.excluido and
                                              not aa.excluido and
                                              not aas.excluido");

            if (!string.IsNullOrEmpty(alunoCodigo))
                query.AppendLine(" and aa.aluno_codigo = @alunoCodigo ");

            if (semestre > 0)
                query.AppendLine(" and aas.semestre = @semestre");

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryAsync<AcompanhamentoAprendizagemAlunoDto>(query.ToString(), new { turmaId, alunoCodigo, semestre });
        }

        public async Task<IEnumerable<AcompanhamentoTurmaAlunoImagemBase64Dto>> ObterInformacoesAcompanhamentoComImagemBase64TurmaAlunos(long turmaId, int semestre, params string[] tagsImagensConsideradas)
        {
            var sqlQuery = new StringBuilder();

            sqlQuery.AppendLine($"select distinct at2.apanhado_geral like '%{tagsImagensConsideradas[0]}%'");
            if (tagsImagensConsideradas.Length > 1)
            {
                for (int i = 1; i < tagsImagensConsideradas.Length; i++)
                    sqlQuery.AppendLine($"                or at2.apanhado_geral like '%{tagsImagensConsideradas[i]}%'");
            }
            sqlQuery.AppendLine("PossuiBase64PercursoTurma,");
            sqlQuery.AppendLine("	   			 acomp_aluno.aluno_codigo CodigoAlunoPercursoPossuiImagemBase64");
            sqlQuery.AppendLine("	from acompanhamento_turma at2");
            sqlQuery.AppendLine("		left join (select aa.turma_id,");
            sqlQuery.AppendLine("		                  aa.aluno_codigo");
            sqlQuery.AppendLine("			          from acompanhamento_aluno aa");
            sqlQuery.AppendLine("			       	     inner join acompanhamento_aluno_semestre aas");
            sqlQuery.AppendLine("			       	        on aa.id = aas.acompanhamento_aluno_id");
            sqlQuery.AppendLine("			       where not aa.excluido and");
            sqlQuery.AppendLine("			             not aas.excluido and");
            sqlQuery.AppendLine($"			             (aas.percurso_individual like '%{tagsImagensConsideradas[0]}%'");
            if (tagsImagensConsideradas.Length > 1)
            {
                for (int i = 1; i < tagsImagensConsideradas.Length; i++)
                    sqlQuery.AppendLine($"			             or aas.percurso_individual like '%{tagsImagensConsideradas[i]}%'");
            }
            sqlQuery.AppendLine(")) acomp_aluno");
            sqlQuery.AppendLine("			on at2.turma_id = acomp_aluno.turma_id");
            sqlQuery.AppendLine("where not at2.excluido and");
            sqlQuery.AppendLine("	   at2.turma_id = @turmaId and");
            sqlQuery.AppendLine("	   at2.semestre = @semestre;");

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryAsync<AcompanhamentoTurmaAlunoImagemBase64Dto>(sqlQuery.ToString(), new { turmaId, semestre });
        }

        public async Task<UltimoSemestreAcompanhamentoGeradoDto> ObterUltimoSemestreAcompanhamentoGerado(string alunoCodigo)
        {
            var query = new StringBuilder(@"SELECT 
                                        max(aa.criado_em)  AS DataCriacao,
                                        aas.semestre 
                                        FROM acompanhamento_aluno aa 
                                        INNER JOIN acompanhamento_aluno_semestre aas  ON aa.id = aas.acompanhamento_aluno_id 
                                        WHERE aa.aluno_codigo = @alunoCodigo
                                        GROUP BY aas.semestre ");

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<UltimoSemestreAcompanhamentoGeradoDto>(query.ToString(), new { alunoCodigo });
            }
        }
    }
}
