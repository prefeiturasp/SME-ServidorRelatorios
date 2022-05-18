using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class DiarioBordoRepository : IDiarioBordoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public DiarioBordoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<AulaDiarioBordoDto>> ObterAulasDiarioBordo(long anoLetivo, int bimestre, string codigoUe, long componenteCurricular, bool listarDataFutura, string codigoTurma, Modalidade modalidadeTurma, ModalidadeTipoCalendario modalidadeCalendario, int semestre)
        {
            var query = $@"select distinct
                              a.id as AulaId
                            , a.data_aula as DataAula
                            , a.aula_cj as AulaCJ
	                        , t.nome as Turma
	                        , coalesce (cc.descricao_infantil, cc.descricao_sgp) as ComponenteCurricular
	                        , pe.bimestre
	                        , db.criado_em as DataPlanejamento
	                        , db.criado_rf as UsuarioRf
	                        , db.criado_por as Usuario
	                        , db.planejamento as Planejamento
	                        , db.devolutiva_id as DevolutivaId
                        from aula a 
                        inner join turma t on t.turma_id = a.turma_id 
                        inner join ue on ue.id = t.ue_id 
                        inner join componente_curricular cc on cc.Id = a.disciplina_id::bigint
                         left join diario_bordo db on db.aula_id  = a.id
                         left join tipo_calendario tc on tc.ano_letivo = @anoLetivo and tc.modalidade = @modalidadeCalendario and not tc.excluido
                         left join periodo_escolar pe on pe.tipo_calendario_id = tc.id and a.data_aula between pe.periodo_inicio and pe.periodo_fim 
                        where t.ano_letivo = @anoLetivo
                          and t.modalidade_codigo = @modalidadeTurma
                          and ue.ue_id = @codigoUe
                          and not a.excluido ";

            if (bimestre != -99)
                query += " and pe.bimestre = @bimestre ";

            if (componenteCurricular != -99)
                query += " and cc.id = @componenteCurricular ";

            if (!listarDataFutura)
                query += " and a.data_aula <= NOW()::DATE ";

            if (codigoTurma != "-99")
                query += " and a.turma_id = @codigoTurma ";

            if (semestre > 0)
                query += " and t.semestre = @semestre ";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryAsync<AulaDiarioBordoDto>(query, new { anoLetivo, bimestre, codigoUe, componenteCurricular, codigoTurma, modalidadeTurma, modalidadeCalendario, semestre });
        }

        public async Task<IEnumerable<AulaDiarioBordoDto>> ObterAulasDiarioBordoComComponenteCurricular(long anoLetivo, int bimestre, string codigoUe, long[] componenteCurriculares, bool listarDataFutura, string codigoTurma, Modalidade modalidadeTurma, ModalidadeTipoCalendario modalidadeCalendario, int semestre)
        {
            var query = $@";with aulas as
                            (
	                            select a.id as aula_id, a.data_aula, a.disciplina_id, t.nome as Turma, pe.bimestre, tipo_aula,a.aula_cj 
	                            from aula a 
	                            join turma t on t.turma_id = a.turma_id 
	                            join ue on ue.id = t.ue_id 
	                            join tipo_calendario tc on tc.ano_letivo = @anoLetivo and tc.modalidade = @modalidadeCalendario and not tc.excluido
	                            join periodo_escolar pe on pe.tipo_calendario_id = tc.id and a.data_aula between pe.periodo_inicio and pe.periodo_fim	
	                            where t.ano_letivo = @anoLetivo
	                              and t.modalidade_codigo = @modalidadeTurma
	                              and ue.ue_id = @codigoUe
	                              and not a.excluido  
                                  {IncluirBimestre(bimestre)}	                                                                  
	                              and a.data_aula <= NOW()::DATE  
                                  {IncluirTurma(codigoTurma)}                                
                            ),
                            componentePai as 
                            (
	                            select cast(disciplina_id as Integer) as disciplina_id from aulas limit 1
                            ),
                            componentesCurriculares as 
                            (
	                            select cc.id as componente_curricular_id, cc.descricao_infantil 
	                            from componente_curricular cc
	                            join componentePai cp on cc.componente_curricular_pai_id = cp.disciplina_id 
	                            where cc.descricao_infantil is not null
                            ),
                            componentesCurricularesDatas as 
                            (
	                            select distinct cc.componente_curricular_id, cc.descricao_infantil, a.data_aula, a.aula_id 
	                            from componentesCurriculares cc 
	                            LEFT JOIN LATERAL (SELECT * from aulas) a ON true
	                            where componente_curricular_id = Any(@componenteCurriculares)
                            )
                            select distinct 
                                  a.aula_id as AulaId,
                                  a.disciplina_id
                                , a.data_aula as DataAula
                                , a.aula_cj as AulaCJ
                                , a.Turma
                                , cc.descricao_infantil as ComponenteCurricular
                                , a.bimestre
                                , db.criado_em as DataPlanejamento
                                , db.criado_rf as UsuarioRf
                                , db.criado_por as Usuario
                                , db.planejamento as Planejamento
                                , db.devolutiva_id as DevolutivaId
                                , a.tipo_aula  as TipoAula
                            from componentesCurricularesDatas cc
                            join aulas a on a.aula_id = cc.aula_id
                            left join diario_bordo db on db.aula_id  = a.aula_id and db.componente_curricular_id = cc.componente_curricular_id 
                            order by a.data_aula,ComponenteCurricular ";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryAsync<AulaDiarioBordoDto>(query, new { anoLetivo, bimestre, codigoUe, componenteCurriculares, codigoTurma, modalidadeTurma, modalidadeCalendario });           
        }

        private string IncluirTurma(string codigoTurma)
        {
            return !string.IsNullOrEmpty(codigoTurma) && (int.Parse(codigoTurma) > 0) ? " and a.turma_id = @codigoTurma " : string.Empty;
        }

        private string IncluirBimestre(int bimestre)
        {
            return bimestre > 0 ? " and pe.bimestre = @bimestre " :  string.Empty;
        }

        public async Task<DateTime?> ObterUltimoDiarioBordoProfessor(string professorRf)
        {
            var query = @"select max(d.criado_em)
                          from aula a 
                          inner join diario_bordo d on d.aula_id = a.id
                         where not a.excluido
                           and a.professor_rf = @professorRf";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            
            return await conexao.QueryFirstOrDefaultAsync<DateTime?>(query, new { professorRf });
        }
    }
}
