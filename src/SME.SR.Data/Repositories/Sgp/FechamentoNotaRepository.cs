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

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<NotaConceitoBimestreComponente>(query, parametros);
            }
        }

        public async Task<IEnumerable<HistoricoAlteracaoNotasDto>> ObterHistoricoAlteracaoNotasFechamento(long turmaId, long tipocalendarioId)
        {
            var query = @"select distinct 
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
                             wan.status as Situacao,
                             wanf.id is not null as EmAprovacao,
                             u.nome as usuarioaprovacao,
                             u.rf_codigo as rfaprovacao
                        from historico_nota hn 
                       inner join historico_nota_fechamento hnf on hn.id = hnf.historico_nota_id 
                       inner join fechamento_nota fn on hnf.fechamento_nota_id = fn.id
                        left join wf_aprovacao_nivel wan on wan.wf_aprovacao_id = hnf.wf_aprovacao_id 
                        left join wf_aprovacao_nivel_notificacao wann on wann.wf_aprovacao_nivel_id = wan.id
                        left join notificacao n on n.id = wann.notificacao_id 
                        left join usuario u on u.id = n.usuario_id
                       inner join fechamento_aluno fa on fn.fechamento_aluno_id = fa.id 
                       inner join fechamento_turma_disciplina ftd on fa.fechamento_turma_disciplina_id = ftd.id 
                       inner join fechamento_turma ft on ftd.fechamento_turma_id = ft.id 
                       inner join periodo_escolar pe on ft.periodo_escolar_id = pe.id
                       inner join componente_curricular cc2 on ftd.disciplina_id = cc2.id
                       left join wf_aprovacao_nota_fechamento wanf on wanf.fechamento_nota_id = fn.id
                           where ft.turma_id = @turmaId
                             and (hnf.wf_aprovacao_id is null or wan.id = (select id from wf_aprovacao_nivel wan2 where wan2.wf_aprovacao_id = wan.wf_aprovacao_id order by wan2.nivel desc limit 1))";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<HistoricoAlteracaoNotasDto>(query, new { turmaId, tipocalendarioId });
            }
        }
    }
}
