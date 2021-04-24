using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<AcompanhamentoAprendizagemAlunoRetornoDto> ObterAcompanhamentoAprendizagemPorTurmaESemestre(long turmaId, string alunoCodigo, int semestre)
        {
            var query = new StringBuilder(@" select aa.id,
                                                   t.id, 
                                                   t.nome as TurmaNome,         
                                                   u.nome as UeNome,
                                                   d.nome as DreNome,
                                                   d.abreviacao as DreAbreviacao,
                                                   aa.aluno_codigo as AlunoCodigo,
                                                   at2.apanhado_geral as ApanhadoGeral,
                                                   aas.observacoes as Observacoes,
                                                   arq.codigo,
                                                   arq.tipo_conteudo as TipoArquivo  
                                              from turma t
                                              inner join ue u on u.id = t.ue_id
                                              inner join dre d on d.id = u.dre_id 
                                              inner join acompanhamento_turma at2 on at2.turma_id = t.id 
                                              inner join acompanhamento_aluno aa on aa.turma_id = t.id 
                                              inner join acompanhamento_aluno_semestre aas on aas.acompanhamento_aluno_id = aa.id 
                                              inner join acompanhamento_aluno_foto aaf on aaf.acompanhamento_aluno_semestre_id = aas.id 
                                              inner join arquivo arq on arq.id = aaf.arquivo_id   
                                              where t.id = @turmaId                                                 
                                                and aaf.miniatura_id is not null ");

            if (string.IsNullOrEmpty(alunoCodigo))            
                query.AppendLine("and aa.aluno_codigo = @alunoCodigo");
            
            if (semestre > 0)
                query.AppendLine("and at2.semestre = @semestre");

            var parametros = new { turmaId, alunoCodigo, semestre };

            var lookup = new Dictionary<long, AcompanhamentoAprendizagemAlunoRetornoDto>();            

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                await conexao.QueryAsync<AcompanhamentoAprendizagemAlunoRetornoDto, AcompanhamentoAprendizagemAlunoFotoDto, AcompanhamentoAprendizagemAlunoRetornoDto > (query.ToString(),
                 (acompanhamentoAprendizagemAlunoRetornoDto, acompanhamentoAprendizagemAlunoFotoDto) =>
                 {
                     AcompanhamentoAprendizagemAlunoRetornoDto acompanhamentoAprendizagem = new AcompanhamentoAprendizagemAlunoRetornoDto();

                     
                     if (!lookup.TryGetValue(acompanhamentoAprendizagemAlunoRetornoDto.Id, out acompanhamentoAprendizagem))
                     {
                         acompanhamentoAprendizagem = acompanhamentoAprendizagemAlunoRetornoDto;
                         lookup.Add(acompanhamentoAprendizagem.Id, acompanhamentoAprendizagemAlunoRetornoDto);
                     }
                     if (acompanhamentoAprendizagemAlunoFotoDto != null)
                         acompanhamentoAprendizagem.Add(acompanhamentoAprendizagemAlunoFotoDto);

                     return acompanhamentoAprendizagem;
                 }, param: parametros, splitOn: "codigo");
             
            }
            return lookup.Values.FirstOrDefault();
        }
    }
}
