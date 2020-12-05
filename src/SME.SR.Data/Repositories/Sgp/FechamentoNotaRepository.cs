using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
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

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<NotaConceitoBimestreComponente>(query, parametros);
            }
        }

        public async Task<IEnumerable<HistoricoAlteracaoNotasDto>> ObterHistoricoAlteracaoNotasFechamento(long turmaId, long tipocalendarioId)
        {
            var query = @"select hn.id as historicoNotaId,
                                 hnf.fechamento_nota_id as fechamentoNotaId,
                                 fa.aluno_codigo as codigoAluno,
                                 hn.nota_anterior as notaAnterior,
                                 hn.nota_nova as notaAtribuida,
                                 hn.conceito_anterior_id as conceitoAnteriorId,
                                 hn.conceito_novo_id as conceitoAtribuidoId, 
                                 hn.criado_por as usuarioAlteracao,
                                 2 as TipoNota,
                                 hn.criado_rf as rfAlteracao,
                                 hn.criado_em as dataAlteracao,
                                 ftd.disciplina_id as disciplinaId,
                                 pe.bimestre,
                                 coalesce(cc2.descricao_sgp,cc2.descricao) as componentecurricularNome,
                                 ap.status_aprovacao as situacao,
                                 ap.usuarioaprovacao,
                                 ap.rfaprovacao
                            from historico_nota hn 
                           inner join historico_nota_fechamento hnf on hn.id = hnf.historico_nota_id 
                           inner join fechamento_nota fn on hnf.fechamento_nota_id = fn.id
                            left join (select wan.status as status_aprovacao,  
				                               coalesce(wan.alterado_por, wan.criado_por) as usuarioaprovacao,
				                               coalesce(wan.alterado_rf, wan.criado_rf) as rfaprovacao,
				                               wanf2.fechamento_nota_id 
				                          from wf_aprovacao_nivel wan
				                         inner join wf_aprovacao wa2 on wan.wf_aprovacao_id = wa2.id 
				                         inner join wf_aprovacao_nota_fechamento wanf2 on wa2.id = wanf2.wf_aprovacao_id 
				                         where wan.wf_aprovacao_id = (select wa.id
								                                        from wf_aprovacao_nota_fechamento wanf
								                                       inner join wf_aprovacao wa on wa.id = wanf.wf_aprovacao_id 
								                                       where wanf.fechamento_nota_id = wanf2.fechamento_nota_id 
								                                         and not excluido limit 1)
				                           and wan.status not in (4, 5)				    
				                         order by wan.nivel desc) ap on ap.fechamento_nota_id = fn.id 
                           inner join fechamento_aluno fa on fn.fechamento_aluno_id = fa.id 
                           inner join fechamento_turma_disciplina ftd on fa.fechamento_turma_disciplina_id = ftd.id 
                           inner join fechamento_turma ft on ftd.fechamento_turma_id = ft.id 
                           inner join periodo_escolar pe on ft.periodo_escolar_id = pe.id
                           inner join componente_curricular cc2 on ftd.disciplina_id = cc2.id   
                           where ft.turma_id = @turmaId
                             and pe.tipo_calendario_id = @tipocalendarioId";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<HistoricoAlteracaoNotasDto>(query, new { turmaId, tipocalendarioId });
            }
        }
    }
}
