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

        public async Task<IEnumerable<RelatorioListagemOcorrenciasRegistroDto>> ObterListagemOcorrenciasAsync(int anoLetivo, string codigoDre, string codigoUe, int modalidade, int semestre, string[] codigosTurma, DateTime? dataInicio, DateTime? dataFim, long[] ocorrenciaTipoIds)
        {
            var query = @"select
	                        o.id as ocorrenciaId,
	                        d.abreviacao as dreabreviacao,
	                        u.ue_id as uecodigo,
	                        u.nome as uenome,
	                        u.tipo_escola as tipoescola,
	                        t.modalidade_codigo as modalidade,
	                        t.nome as turmaNome,
	                        t.tipo_turno as tipoTurno,
	                        o.data_ocorrencia as dataOcorrencia,
	                        ot.descricao as ocorrenciaTipo,
	                        o.titulo,
	                        o.descricao 
                        from ocorrencia o
                        left join ocorrencia_tipo ot on ot.id = o.ocorrencia_tipo_id 
                        left join turma t on t.id = o.turma_id
                        left join ue u on u.id = t.ue_id
                        left join dre d on d.id = u.dre_id
                        where not o.excluido and t.ano_letivo = @anoLetivo";

            if (codigoDre != "-99")
                query += " and d.dre_id = @codigoDre ";

            if (codigoUe != "-99")
                query += " and u.ue_id = @codigoUe ";

            if (codigoUe != "-99")
                query += " and u.ue_id = @codigoUe ";

            if (modalidade != -99)
                query += " and t.modalidade_codigo = @modalidade ";

            if (semestre > 0)
                query += " and t.semestre = @semestre ";

            if (!codigosTurma.Contains("-99"))
                query += " and t.turma_id = any(@codigosTurma) ";

            if (dataInicio.HasValue && dataFim.HasValue)
                query += " and o.data_ocorrencia between @dataInicio and @dataFim ";

            if (!ocorrenciaTipoIds.Contains(-99))
                query += " and o.ocorrencia_tipo_id = any(@ocorrenciaTipoIds) ";

            var parametros = new
            {
                anoLetivo,
                codigoDre,
                codigoUe,
                modalidade,
                semestre,
                codigosTurma,
                dataInicio = dataInicio.GetValueOrDefault().Date,
                dataFim = dataFim.GetValueOrDefault().Date,
                ocorrenciaTipoIds
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<RelatorioListagemOcorrenciasRegistroDto>(query, parametros);
            }
        }

        public async Task<IEnumerable<RelatorioListagemOcorrenciasRegistroAlunoDto>> ObterAlunosOcorrenciasPorIdsAsync(int[] ocorrenciaIds)
        {
            var query = @"select 
                            oa.ocorrencia_id as ocorrenciaId,
                            oa.codigo_aluno as codigoAluno
                          from ocorrencia_aluno oa 
                          where oa.ocorrencia_id = any(@ocorrenciaIds)";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<RelatorioListagemOcorrenciasRegistroAlunoDto>(query, new { ocorrenciaIds });
            }
        }

        public async Task<IEnumerable<RelatorioListagemOcorrenciasRegistroServidorDto>> ObterServidoresOcorrenciasPorIds(int[] ocorrenciaIds)
        {
            var query = @"select 
                                os.ocorrencia_id as ocorrenciaId,
                                os.rf_codigo as codigoRF
                          from ocorrencia_servidor os 
                          where os.ocorrencia_id = any(@ocorrenciaIds)";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<RelatorioListagemOcorrenciasRegistroServidorDto>(query, new { ocorrenciaIds });
            }
        }
    }
}
