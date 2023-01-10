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
    public class OcorrenciaRepository : IOcorrenciaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public OcorrenciaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new System.ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<AcompanhamentoAprendizagemOcorrenciaDto>> ObterOcorenciasPorTurmaEAluno(long turmaId, long? alunoCodigo, DateTime dataInicio, DateTime dataFim)
        {
            var query = new StringBuilder(@"select oa.codigo_aluno as AlunoCodigo, 
       		                                       o.turma_id as TurmaId,
       		                                       o.data_ocorrencia as DataOcorrencia, 
       		                                       o.hora_ocorrencia as HoraOcorrencia, 
       		                                       o.titulo as TituloOcorrencia,
       		                                       o.descricao as Descricao, 
       		                                       ot.descricao TipoOcorrencia
                                              from ocorrencia o 
                                             inner join ocorrencia_aluno oa on oa.ocorrencia_id = o.id 
                                             inner join ocorrencia_tipo ot on ot.id = o.ocorrencia_tipo_id 
                                             where o.turma_id = @turmaId
                                               and o.data_ocorrencia::date between @dataInicio and @dataFim
                                               and not o.excluido ");

            if (alunoCodigo != null && alunoCodigo > 0)
                query.AppendLine("and oa.codigo_aluno = @alunoCodigo");

            var parametros = new 
            { 
                turmaId, 
                alunoCodigo,
                dataInicio,
                dataFim
            };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryAsync<AcompanhamentoAprendizagemOcorrenciaDto>(query.ToString(), parametros);
        }

        public async Task<IEnumerable<OcorrenciasPorCodigoTurmaDto>> ObterOcorrenciasCodigoETurma(long turmaId, long[] ocorrenciaIds)
        {
            var query = @"select
                        	o.id as OcorrenciaId, 
                        	o.turma_id as TurmaId,
                        	o.titulo as OcorrenciaTitulo,
                        	TO_CHAR(o.data_ocorrencia,'dd/MM/yyyy') ||' '||TO_CHAR(o.hora_ocorrencia ,'HH24:MI')|| 'h' as OcorrenciaData,
                        	o.descricao as OcorrenciaDescricaoComTagsHtml,
                        	ot.descricao as OcorrenciaTipo,
							oa.codigo_aluno as CodigoAluno
                        from  ocorrencia o
						inner join ocorrencia_tipo ot on ot.id = o.ocorrencia_tipo_id 
						inner join ocorrencia_aluno oa on oa.ocorrencia_id = o.id 
                        where not o.excluido 
                        	and o.id = any(@ocorrenciaIds)";

            if (turmaId != 0)
                query += " and o.turma_id = @turmaId ";
            query += " order by o.data_ocorrencia desc";

            var parametros = new
            {
                turmaId,
                ocorrenciaIds
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<OcorrenciasPorCodigoTurmaDto>(query, parametros);                
            }
        }
    }
}
