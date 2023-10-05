using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class AulaPrevistaBimestreRepository : IAulaPrevistaBimestreRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public AulaPrevistaBimestreRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<AulaPrevistaBimestreQuantidade>> ObterBimestresAulasPrevistasPorFiltro(long turmaId, long componenteCurricularId, long tipoCalendarioId)
        {
            var query = @"
                select t.nome as TurmaNome, @componenteCurricularId as ComponenteCurricularId, coalesce(cc.descricao_sgp, cc.descricao) as ComponenteCurricularNome, cc.eh_regencia as Regencia
	                , p.bimestre, p.periodo_inicio as DataInicio, p.periodo_fim as DataFim
                    , pv.aulas_previstas as Previstas
                    , SUM(ac.quantidade) filter (where ac.tipo_aula = 1 and ac.aula_cj = false) as CriadasTitular
                    , SUM(ac.quantidade) filter (where ac.tipo_aula = 1 and ac.aula_cj = true) as CriadasCJ
                    , SUM(ac.quantidade) filter (where ac.tipo_aula = 1 and ac.registro_frequencia_id is not null and ac.aula_cj = false) as CumpridasTitular
                    , SUM(ac.quantidade) filter (where ac.tipo_aula = 1 and ac.registro_frequencia_id is not null and ac.aula_cj = true) as CumpridasCj
                    , SUM(ac.quantidade) filter (where ac.tipo_aula = 2 and ac.registro_frequencia_id is not null ) as Reposicoes 
                  from periodo_escolar p
                 inner join turma t on t.id = @turmaId
                 left join componente_curricular cc on cc.id = @componenteCurricularId
                  left join (
  	                select distinct ap.tipo_calendario_id, ap.disciplina_id, ap.turma_id, apb.bimestre, apb.aulas_previstas
  	                  from aula_prevista ap
  	                 inner join aula_prevista_bimestre apb on apb.aula_prevista_id = ap.id
                     where not ap.excluido and not apb.excluido
                  ) pv on pv.tipo_calendario_id = p.tipo_calendario_id
                     and pv.bimestre = p.bimestre
                     and pv.disciplina_id = @componenteCurricularId::text
                     and pv.turma_id = t.turma_id
                   left join (
	                select a.tipo_calendario_id, a.turma_id, a.disciplina_id, a.data_aula, a.quantidade, a.tipo_aula, a.aula_cj, rf.id as registro_frequencia_id
	                  from aula a
  	                  left join registro_frequencia rf on a.id = rf.aula_id  
	                 where not a.excluido
                   ) ac on ac.tipo_calendario_id = p.tipo_calendario_id
                     and ac.turma_id = t.turma_id
                     and ac.disciplina_id = @componenteCurricularId::text
                     and ac.data_aula between p.periodo_inicio and p.periodo_fim
                 where p.tipo_calendario_id = @tipoCalendarioId
                group by t.nome, @componenteCurricularId, cc.descricao_sgp, cc.descricao, cc.eh_regencia, p.bimestre, p.periodo_inicio, p.periodo_fim, pv.aulas_previstas ";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<AulaPrevistaBimestreQuantidade>(query.ToString(), new { turmaId, componenteCurricularId, tipoCalendarioId });
            }            
        }

        public async Task<IEnumerable<TurmaComponenteQuantidadeAulasDto>> ObterBimestresAulasTurmasComponentesCumpridasAsync(string[] turmasCodigos, string[] componentesCurricularesId, long tipoCalendarioId)
        {
            var query = new StringBuilder(@"select
                            p.id PeriodoEscolarId,
	                        p.bimestre,
	                        ap.turma_id as TurmaCodigo,
	                        ap.disciplina_id as ComponenteCurricularCodigo,	
	                        SUM(a.quantidade) filter (
	                        where a.tipo_aula = 1
	                        and rf.id is not null) as AulasQuantidade                            
                        from
	                        periodo_escolar p
                        inner join tipo_calendario tp on
	                        p.tipo_calendario_id = tp.id
                        left join aula_prevista ap on
	                        ap.tipo_calendario_id = p.tipo_calendario_id
                        left join aula_prevista_bimestre apb on
	                        ap.id = apb.aula_prevista_id
	                        and p.bimestre = apb.bimestre
                            and not apb.excluido
                        left join aula a on
	                        a.turma_id = ap.turma_id
	                        and a.disciplina_id = ap.disciplina_id
	                        and a.tipo_calendario_id = p.tipo_calendario_id
	                        and a.data_aula between p.periodo_inicio and p.periodo_fim
	                        and (a.id is null
	                        or not a.excluido)
                        left join registro_frequencia rf on
	                        a.id = rf.aula_id
                        where
	                        tp.situacao
	                        and not tp.excluido
	                        and ap.tipo_calendario_id = @tipoCalendarioId
	                        and ap.turma_id = ANY(@turmasCodigos)
	                        and ap.disciplina_id = ANY(@componentesCurricularesId)");
            query.AppendLine(@" group by
                            p.id,
	                        p.bimestre,
	                        ap.turma_id,
	                        ap.disciplina_id;");

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
                return (await conexao.QueryAsync<TurmaComponenteQuantidadeAulasDto>(query.ToString(), new {  turmasCodigos, componentesCurricularesId, tipoCalendarioId }));
        }
    }
}
