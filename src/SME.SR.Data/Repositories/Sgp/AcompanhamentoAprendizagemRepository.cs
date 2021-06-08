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
            var query = new StringBuilder(@"select at2.id,
   	        	                                   at2.apanhado_geral as ApanhadoGeral,
   	        	                                   at2.semestre,
   	                                               tb1.id,                                                   
                                                   tb1.aluno_codigo as AlunoCodigo,                   
                                                   tb1.observacoes as Observacoes,  
                                                   tb1.arquivoId as Id,
                                                   tb1.codigo,
                                                   tb1.nome as NomeOriginal,
                                                   tb1.tipo_conteudo as TipoArquivo,
                                                   tb1.tipo
                                              from acompanhamento_turma at2
                                              left join (select aa.id,
      					                                        aa.turma_id,
      					                                        aas.semestre,
			       		                                        aa.aluno_codigo,       		
			       		                                        aas.observacoes,  
                                                                arq.id ArquivoId,
			       		                                        arq.codigo,
			       		                                        arq.nome,
			       		                                        arq.tipo_conteudo, arq.tipo
			                                               from acompanhamento_aluno aa
			                                              inner join acompanhamento_aluno_semestre aas on aas.acompanhamento_aluno_id = aa.id
			   	                                           left join acompanhamento_aluno_foto aaf on aaf.acompanhamento_aluno_semestre_id = aas.id 
			   	                                           left join arquivo arq on arq.id = aaf.arquivo_id
			   	                                          where aa.turma_id = @turmaId ");

            if (!string.IsNullOrEmpty(alunoCodigo))
                query.AppendLine("and aa.aluno_codigo = @alunoCodigo ");

            query.AppendLine(@"and aaf.miniatura_id is not null) as tb1 on tb1.turma_id = at2.turma_id and tb1.semestre = at2.semestre
                               where at2.turma_id = @turmaId ");

            if (semestre > 0)
                query.AppendLine("and at2.semestre = @semestre");

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
