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
    public class FechamentoNotaRepository : IFechamentoNotaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public FechamentoNotaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<NotaConceitoBimestreComponente>> ObterNotasAlunoBimestre(long fechamentoTurmaId, string codigoAluno)
        {
            var query = FechamentoNotaConsultas.NotasAlunoBimestre;
            var parametros = new { FechamentoTurmaId = fechamentoTurmaId, CodigoAluno = codigoAluno};

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<NotaConceitoBimestreComponente>(query, parametros);
            }
        }

        public async Task<IEnumerable<HistoricoAlteracaoNotasDto>> ObterHistoricoAlteracaoNotasFechamento(long turmaId, long tipocalendarioId, int[] bimestres, long[] componentes)
        {
	        var criterios = new StringBuilder();
	        
	        if (bimestres.Contains(0))
		        criterios.AppendLine(@" and ft.periodo_escolar_id is null ");

	        if (!bimestres.Contains(-99) && !bimestres.Contains(0))
		        criterios.AppendLine(@" and pe.tipo_calendario_id = @tipocalendarioId and pe.bimestre = ANY(@bimestres) ");
            
	        if (componentes.Length > 0)
		        criterios.AppendLine(@" and ftd.disciplina_id = ANY(@componentes)");
	        
	        var query = new StringBuilder(@$"with vw_notas as (
												 select distinct 
												     fa.aluno_codigo as codigoAluno,
												     fn.nota as notaAnterior,
												     wanf.nota as notaAtribuida,
												     fn.conceito_id as conceitoAnteriorId,
												     wanf.conceito_id as conceitoAtribuidoId, 
												     wanf.criado_por as usuarioAlteracao,
												     2 as TipoNota,
												     wanf.criado_rf as rfAlteracao,
												     wanf.criado_em as dataAlteracao,
												     fn.disciplina_id as disciplinaId,
												     pe.bimestre,
												     coalesce(cc2.descricao_sgp,cc2.descricao) as componentecurricularNome,
												     wan.status as Situacao,
												     wanf.id is not null as EmAprovacao,     
												     u.nome as usuarioaprovacao,
												     u.rf_codigo as rfaprovacao
												from wf_aprovacao_nota_fechamento wanf
													join fechamento_nota fn on wanf.fechamento_nota_id = fn.id
												    join fechamento_aluno fa on fn.fechamento_aluno_id = fa.id 
												    join fechamento_turma_disciplina ftd on fa.fechamento_turma_disciplina_id = ftd.id 
												    join fechamento_turma ft on ftd.fechamento_turma_id = ft.id                        
												    join turma t on ft.turma_id = t.id
												    join componente_curricular cc2 on fn.disciplina_id = cc2.id --ftd.disciplina_id = cc2.id
												    left join periodo_escolar pe on ft.periodo_escolar_id = pe.id
													left join wf_aprovacao wa on wa.id = wanf.wf_aprovacao_id
												    left join wf_aprovacao_nivel wan on wan.wf_aprovacao_id = wanf.wf_aprovacao_id  
												    left join wf_aprovacao_nivel_notificacao wann on wann.wf_aprovacao_nivel_id = wan.id                                                     
												    left join notificacao n on n.id = wann.notificacao_id and n.turma_id = t.turma_id
												    left join usuario u on u.id = n.usuario_id 
												 where ft.turma_id = @turmaId
													   and not wanf.excluido	 												   
													   and (wanf.wf_aprovacao_id is null  or wan.id = (select id from wf_aprovacao_nivel wan2 where wan2.wf_aprovacao_id = wan.wf_aprovacao_id order by wan2.nivel desc limit 1))
													   {criterios}	  
											 union all
												 select distinct 
												     fa.aluno_codigo as codigoAluno,
												     hn.nota_anterior as notaAnterior,
												     hn.nota_nova as notaAtribuida,
												     hn.conceito_anterior_id as conceitoAnteriorId,
												     hn.conceito_novo_id as conceitoAtribuidoId, 
												     hn.criado_por as usuarioAlteracao,
												     2 as TipoNota,
												     hn.criado_rf as rfAlteracao,
												     hn.criado_em as dataAlteracao,
												     fn.disciplina_id as disciplinaId,
												     pe.bimestre,
												     coalesce(cc2.descricao_sgp,cc2.descricao) as componentecurricularNome,
												     0 as Situacao,
												     false as EmAprovacao,     
												     '' as usuarioaprovacao,
												     '' as rfaprovacao
												from historico_nota hn 
												   inner join historico_nota_fechamento hnf on hn.id = hnf.historico_nota_id 
												   inner join fechamento_nota fn on hnf.fechamento_nota_id = fn.id
												   inner join fechamento_aluno fa on fn.fechamento_aluno_id = fa.id 
												   inner join fechamento_turma_disciplina ftd on fa.fechamento_turma_disciplina_id = ftd.id 
												   inner join fechamento_turma ft on ftd.fechamento_turma_id = ft.id                        
												   inner join turma t on ft.turma_id = t.id 
												   left join periodo_escolar pe on ft.periodo_escolar_id = pe.id
												   inner join componente_curricular cc2 on fn.disciplina_id = cc2.id  
												 where ft.turma_id = @turmaId 
													   {criterios}
								) select * from vw_notas order by dataAlteracao,codigoAluno desc ");

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<HistoricoAlteracaoNotasDto>(query.ToString(), new { turmaId, tipocalendarioId, bimestres, componentes });
            }
        }
    }
}
