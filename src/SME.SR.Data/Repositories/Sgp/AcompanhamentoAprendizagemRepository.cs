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

        public async Task<IEnumerable<AcompanhamentoAprendizagemTurmaDto>> ObterAcompanhamentoAprendizagemPorTurmaESemestre(long turmaId, string alunoCodigo, int semestre)
        {
            var query = new StringBuilder(@"SELECT tb1.id,
	                                               tb1.ApanhadoGeral,
	                                               tb1.semestre,
	                                               aa.id,
                                                   aa.aluno_codigo AS AlunoCodigo,
                                                   aas.observacoes AS Observacoes,
                                                   aas.percurso_individual AS PercursoIndividual,
                                                   arq.id as Id,
                                                   arq.codigo,
                                                   arq.nome AS NomeOriginal,
                                                   arq.tipo_conteudo AS TipoArquivo,
                                                   arq.tipo
                                            FROM   acompanhamento_aluno aa
                                                   INNER JOIN acompanhamento_aluno_semestre aas
                                                           ON aas.acompanhamento_aluno_id = aa.id
                                                   LEFT JOIN acompanhamento_aluno_foto aaf
                                                          ON aaf.acompanhamento_aluno_semestre_id = aas.id
                                                   LEFT JOIN arquivo arq
                                                          ON arq.id = aaf.arquivo_id  AND aaf.miniatura_id IS NOT null
                                                   left join (select atr.id, 
       				                                               atr.apanhado_geral AS ApanhadoGeral, 
       				                                               atr.semestre,
       				                                               atr.turma_id
       		                                                  from acompanhamento_turma atr) as tb1 ON tb1.turma_id = aa.turma_id AND tb1.semestre = aas.semestre
                                            WHERE aa.turma_id = @turmaId ");

            if (!string.IsNullOrEmpty(alunoCodigo))
                query.AppendLine("and aa.aluno_codigo = @alunoCodigo ");

            if (semestre > 0)
                query.AppendLine("AND aas.semestre = @semestre");

            var parametros = new { turmaId, alunoCodigo, semestre };

            var lookup = new Dictionary<long, AcompanhamentoAprendizagemTurmaDto>();


            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                await conexao.QueryAsync<AcompanhamentoAprendizagemTurmaDto, AcompanhamentoAprendizagemAlunoDto, ArquivoDto, AcompanhamentoAprendizagemTurmaDto>(query.ToString(),
                 (acompanhamentoAprendizagemTurmaDto, acompanhamentoAprendizagemAlunoDto, arquivoDto) =>
                 {
                     AcompanhamentoAprendizagemTurmaDto acompanhamentoAprendizagem = new AcompanhamentoAprendizagemTurmaDto();

                     if (!lookup.TryGetValue(acompanhamentoAprendizagemTurmaDto.Id, out acompanhamentoAprendizagem))
                     {
                         acompanhamentoAprendizagem = acompanhamentoAprendizagemTurmaDto;
                         lookup.Add(acompanhamentoAprendizagem.Id, acompanhamentoAprendizagemTurmaDto);
                     }
                     if (acompanhamentoAprendizagemAlunoDto != null)
                         acompanhamentoAprendizagem.Add(acompanhamentoAprendizagemAlunoDto);

                     if (arquivoDto != null)
                         acompanhamentoAprendizagem.AddFotoAluno(acompanhamentoAprendizagemAlunoDto.AlunoCodigo, arquivoDto);

                     return acompanhamentoAprendizagem;
                 }, param: parametros);

            }

            return lookup.Values;
        }
    }
}
