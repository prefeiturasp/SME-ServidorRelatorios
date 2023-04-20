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
    public class AulaRepository : IAulaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public AulaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<int> ObterAulasDadas(string codigoTurma, string[] componentesCurricularesCodigo, long tipoCalendarioId, int bimestre)
        {
            var query = new StringBuilder(@"
                        select SUM(a.quantidade) AulasQuantidade
                        	from periodo_escolar pe 
                        		inner join tipo_calendario tc 
                        			on pe.tipo_calendario_id = tc.id 
                        		inner join aula a 
                        			on tc.id = a.tipo_calendario_id		
                        where tc.situacao and
                              tc.id = @tipoCalendarioId and
                        	  not tc.excluido and
                        	  pe.bimestre = @bimestre and
                        	  a.data_aula::date between pe.periodo_inicio and pe.periodo_fim and
                        	  a.disciplina_id = any(@componentesCurricularesCodigo) and
                        	  a.turma_id = @codigoTurma and
                        	  exists (select 1
                        	  		  	from registro_frequencia_aluno rfa 
                        	  		  where not rfa.excluido and
                        	  		  	    rfa.aula_id = a.id);");
          

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
                return await conexao.QuerySingleOrDefaultAsync<int>(query.ToString(), new { tipoCalendarioId, codigoTurma, componentesCurricularesCodigo, bimestre });
        }

        public async Task<AulaPrevista> ObterAulaPrevistaFiltro(long tipoCalendarioId, string turmaId, string disciplinaId)
        {
            var query = @"select * from aula_prevista ap
                         where ap.tipo_calendario_id = @tipoCalendarioId and ap.turma_id = @turmaId and
                               ap.disciplina_id = @disciplinaId;";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<AulaPrevista>(query, new { tipoCalendarioId, turmaId, disciplinaId });
            }
        }

        public async Task<bool> VerificaExisteAulaCadastrada(long turmaId, string componenteCurricularId, int bimestre, long tipoCalendarioId)
        {
            var query = @"select distinct 1 from aula a inner join turma on a.turma_id = turma.turma_id 
                inner join periodo_escolar p on p.tipo_calendario_id = a.tipo_calendario_id and a.data_aula between p.periodo_inicio and p.periodo_fim
                where turma.id = @turmaId and a.disciplina_id = @componenteCurricularId and a.tipo_calendario_id = @tipoCalendarioId and p.bimestre = @bimestre;";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<bool>(query, new { turmaId, componenteCurricularId, bimestre, tipoCalendarioId });
            }
        }

        public async Task<bool> VerificaExisteAulaCadastradaProfessorRegencia(string componenteCurricularId, int bimestre, long tipoCalendarioId)
        {
            var query = @"select 1
                           from aula a
                          inner join turma t on t.turma_id = a.turma_id
                          inner join periodo_escolar pe on a.data_aula between pe.periodo_inicio and pe.periodo_fim
                          where not a.excluido
                            and a.tipo_aula = 1
                            and a.disciplina_id::bigint = @componenteCurricularId
                            and t.id = :turmaId
                            and pe.tipo_calendario_id = @tipoCalendarioId
                            and pe.bimestre = @bimestre
                          group by a.data_aula, a.professor_rf
                         having sum(a.quantidade) >= 2";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return (await conexao.QueryAsync<int>(query, new { componenteCurricularId, bimestre, tipoCalendarioId })).Any();
            }
        }
       
        public async Task<bool> VerificaExisteMaisAulaCadastradaNoDia(long turmaId, string componenteCurricularId, long tipoCalendarioId, int bimestre)
        {
            var query = @"select distinct 1 
                            from aula a 
                          inner join periodo_escolar p on p.tipo_calendario_id = a.tipo_calendario_id and a.data_aula between p.periodo_inicio and p.periodo_fim
                          inner join turma on a.turma_id = turma.turma_id 
                          where not a.excluido
                            and turma.id = @turmaId
                            and a.tipo_calendario_id = @tipoCalendarioId
                            and a.disciplina_id = @componenteCurricularId 
                            and p.bimestre = @bimestre 
                         group by a.data_aula, a.tipo_aula, a.criado_rf having count(a.id) > 1";

            var parametros = new
            {
                componenteCurricularId,
                turmaId,
                tipoCalendarioId,
                bimestre
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<bool>(query, parametros);
            }
        }

        public async Task<bool> VerificaExsiteAulaTitularECj(long turmaId, long componenteCurricularId, long tipoCalendarioId, int bimestre)
        {
            var query = @" select 1
                               from aula a 
                              inner join turma t on t.turma_id = a.turma_id
                              inner join periodo_escolar pe on a.data_aula between pe.periodo_inicio and pe.periodo_fim
                              where not a.excluido
                                and a.disciplina_id::bigint = @componenteCurricularId
                                and t.id = @turmaId
                                and pe.tipo_calendario_id = @tipoCalendarioId
                                and pe.bimestre = @bimestre
                              group by a.data_aula
                            having count(distinct a.aula_cj) > 1";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return (await conexao.QueryAsync<int>(query, new { turmaId, componenteCurricularId, tipoCalendarioId, bimestre })).Any();
            }
        }

        public async Task<bool> VerificaExisteAulasMesmoDiaProfessor(long turmaId, long componenteCurricularId, long tipoCalendarioId, int bimestre)
        {
            var query = @" select 1
                           from aula a 
                          inner join turma t on t.turma_id = a.turma_id
                          inner join periodo_escolar pe on a.data_aula between pe.periodo_inicio and pe.periodo_fim
                          where not a.excluido
                            and a.tipo_aula = 1
                            and a.disciplina_id::bigint = @componenteCurricularId
                            and t.id = @turmaId
                            and pe.tipo_calendario_id = @tipoCalendarioId
                            and pe.bimestre = @bimestre
                          group by a.data_aula, a.professor_rf
                        having sum(a.quantidade) >= 3 ";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return (await conexao.QueryAsync<int>(query, new { turmaId, componenteCurricularId, tipoCalendarioId, bimestre })).Any();
            }
        }

        public async Task<IEnumerable<AulaDuplicadaControleGradeDto>> DetalharAulasDuplicadasNoDia(long turmaId, string componenteCurricularId, long tipoCalendarioId, int bimestre)
        {
            var query = @"select to_char(a.data_aula, 'dd/MM/yyyy') as data, a.criado_por  || ' ('  || a.criado_rf  || ')' as Professor, count(a.quantidade) as QuantidadeDuplicado
                            from aula a 
                          inner join periodo_escolar p on p.tipo_calendario_id = a.tipo_calendario_id and a.data_aula between p.periodo_inicio and p.periodo_fim
                          inner join turma on a.turma_id = turma.turma_id 
                          where not a.excluido
                            and turma.id = @turmaId
                            and a.tipo_calendario_id = @tipoCalendarioId
                            and a.disciplina_id = @componenteCurricularId
                            and p.bimestre = @bimestre
                         group by a.data_aula, a.tipo_aula, a.criado_por, a.criado_rf 
                         having count(a.quantidade) > 1";

            var parametros = new
            {
                componenteCurricularId,
                turmaId,
                tipoCalendarioId,
                bimestre
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<AulaDuplicadaControleGradeDto>(query, parametros);
            }
        }

        public async Task<IEnumerable<AulaNormalExcedidoControleGradeDto>> ObterAulasExcedidas(long turmaId, string componenteCurricularId, long tipoCalendarioId, int bimestre)
        {
            var query = @"select
	                        TO_CHAR(a.data_aula,'dd/MM/YYYY') as Data,
	                        sum(a.quantidade) as QuantidadeAulas,
	                        a.criado_por as Professor,
                            a.criado_rf as ProfessorRf
                        from
	                        aula a
                        inner join turma t on
	                        a.turma_id = t.turma_id
                        inner join componente_curricular cc on
	                        a.disciplina_id = cc.id::varchar
                        inner join periodo_escolar pe on
	                        a.data_aula between pe.periodo_inicio and pe.periodo_fim
                        where
	                        disciplina_id = @componenteCurricularId
	                        and t.id = @turmaId
	                        and pe.bimestre = @bimestre
	                        and not a.excluido 
                            and pe.tipo_calendario_id = @tipoCalendarioId
                       group by
	                        a.data_aula,
	                        a.criado_por,
                            a.criado_rf,
	                        cc.eh_regencia
                        having (case when cc.eh_regencia then sum(a.quantidade) >= 2 else sum(a.quantidade) >= 3 end)
                        order by
	                        data_aula";
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return (await conexao.QueryAsync<AulaNormalExcedidoControleGradeDto>(query, new { turmaId, componenteCurricularId, tipoCalendarioId, bimestre }));
            }
        }

        public async Task<IEnumerable<AulaReduzidaDto>> ObterQuantidadeAulasReduzido(long turmaId, string componenteCurricularId, long tipoCalendarioId, int bimestre, bool professorCJ)
        {
            var query = @"select
	                        a.data_aula as Data,
	                        sum(a.quantidade) as Quantidade,
	                        a.criado_por as Professor,
	                        a.criado_rf as ProfessorRf
                        from
	                        aula a
                        inner join turma t on
	                        a.turma_id = t.turma_id
                        inner join periodo_escolar pe on
	                        a.data_aula between pe.periodo_inicio and pe.periodo_fim
                        where
	                        disciplina_id = @componenteCurricularId
	                        and t.id = @turmaId
	                        and pe.bimestre = @bimestre
	                        and not a.excluido 
                            and pe.tipo_calendario_id = @tipoCalendarioId
                            and a.aula_cj = @professorCJ
                        group by
	                        a.data_aula,
	                        a.criado_por,
                            a.criado_por,
                            a.criado_rf
                        order by
	                        data_aula";
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return (await conexao.QueryAsync<AulaReduzidaDto>(query, new { turmaId, componenteCurricularId, tipoCalendarioId, bimestre, professorCJ }));
            }
        }

        public async Task<int> ObterQuantidadeAulaGrade(long turmaId, long componenteCurricularId)
        {
            var query = @"select
	                        quantidade_aulas as quantidadeGrade
                        from
	                        grade_filtro gf
                        inner join turma t
	                        on gf.duracao_turno = t.qt_duracao_aula and gf.modalidade = t.modalidade_codigo 
                        inner join ue u 
	                        on t.ue_id = u.id and gf.tipo_escola = u.tipo_escola 
                        inner join grade_disciplina gd on
	                        gf.grade_id = gd.grade_id and t.ano = gd.ano::varchar
                        where
	                        t.id = @turmaId and 
	                        gd.componente_curricular_id = @componenteCurricularId ";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return (await conexao.QueryFirstOrDefaultAsync<int>(query, new { turmaId, componenteCurricularId }));
            }
        }

        public async Task<int> ObterDiasAulaCriadasPeriodoInicioEFim(long turmaId, long componenteCurricularId, DateTime dataInicio, DateTime dataFim)
        {
            var query = @"select coalesce(sum(a.quantidade),0) from aula a
	                        inner join turma t on a.turma_id = t.turma_id 
	                        where 
		                        disciplina_id = @componenteCurricularId::varchar and 
		                        data_aula between @dataInicio and @dataFim
                                and t.id = @turmaId
		                        and not a.excluido 
		                        and a.tipo_aula = 1";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return (await conexao.QueryFirstOrDefaultAsync<int>(query, new { turmaId, componenteCurricularId, dataInicio, dataFim }));

            }
        }

        public async Task<int> ObterQuantidadeAulaCriadasPeriodoInicioEFim(long turmaId, long componenteCurricularId, DateTime dataInicio, DateTime dataFim)
        {
            var query = @"select coalesce(sum(a.quantidade),0) from aula a
	                        inner join turma t on a.turma_id = t.turma_id 
	                        where 
		                        disciplina_id = @componenteCurricularId::varchar and 
		                        data_aula between @dataInicio and @dataFim
                                and t.id = @turmaId
		                        and not a.excluido 
		                        and a.tipo_aula = 1";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return (await conexao.QueryFirstOrDefaultAsync<int>(query, new { turmaId, componenteCurricularId, dataInicio, dataFim }));

            }
        }

        public async Task<DateTime?> ObterUltimaAulaCadastradaProfessor(string professorRf)
        {
            var query = @"select max(criado_em) from aula where not excluido and professor_rf = @professorRf";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<DateTime?>(query, new { professorRf });
            }
        }

        public async Task<IEnumerable<AulaReduzidaDto>> ObterAulasReduzido(long[] turmasId, string[] componenteCurricularesId, bool professorCJ)
        {
            var query = @"select
	                        a.data_aula as Data,
	                        a.criado_por as Professor,
	                        a.criado_rf as ProfessorRf
                        from
	                        aula a
                        inner join turma t on
	                        a.turma_id = t.turma_id
                        inner join periodo_escolar pe on
	                        a.data_aula between pe.periodo_inicio and pe.periodo_fim
                        where
	                        disciplina_id = ANY(@componenteCurricularesId)
	                        and t.id = ANY(@turmasId)
	                        and not a.excluido 
                            and a.aula_cj = @professorCJ
                        group by
	                        a.data_aula,
	                        a.criado_por,
                            a.criado_por,
                            a.criado_rf
                        order by
	                        data_aula";
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return (await conexao.QueryAsync<AulaReduzidaDto>(query, new { turmasId, componenteCurricularesId, professorCJ }));
            }
        }

        public async Task<IEnumerable<AulaVinculosDto>> ObterAulasVinculos(string[] turmasId, string[] componenteCurricularesId, bool professorCJ)
        {
            var query = @"select
	                        a.data_aula as Data,
	                        a.criado_por as Professor,
	                        a.criado_rf as ProfessorRf,
                            a.turma_id as TurmaCodigo,
                            a.disciplina_id as ComponenteCurricularId,
                            cc.permite_registro_frequencia ControlaFrequencia,
                           CASE
						        WHEN rf.id is not null THEN 1
						        ELSE 0
						    END  FrequenciaRegistrada,
                            CASE
						        WHEN aa.id is not null THEN 1
						        ELSE 0
						    END PossuiAtividadeAvaliativa,
					        CASE
						        WHEN n.id is not null THEN 1
						        ELSE 0
						    END PossuiNotaLancada       
                        from
	                        aula a
                        inner join turma t on
	                        a.turma_id = t.turma_id
                        inner join componente_curricular cc on a.disciplina_id = cc.id::varchar
                        left join registro_frequencia rf on rf.aula_id = a.id
                        left join atividade_avaliativa aa on
                        	aa.turma_id = a.turma_id
	                        and aa.data_avaliacao::date = a.data_aula::date
                        left join atividade_avaliativa_disciplina aad on
	                        aad.atividade_avaliativa_id = aa.id 
	                        and aad.disciplina_id = a.disciplina_id 
	                    left join notas_conceito n on
                		    aa.id = n.atividade_avaliativa
                        where
	                        a.disciplina_id = ANY(@componenteCurricularesId)
	                        and t.turma_id = ANY(@turmasId)
	                        and not a.excluido 
                            and a.aula_cj = @professorCJ
                        group by 	
	                        a.data_aula,
	                        a.criado_por,
	                        a.criado_rf,
                            a.turma_id,
                            a.disciplina_id,
                            cc.permite_registro_frequencia,
	                    	FrequenciaRegistrada,
	                    	PossuiAtividadeAvaliativa,
	                    	PossuiNotaLancada
                        order by
	                        data_aula";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return (await conexao.QueryAsync<AulaVinculosDto>(query, new { turmasId, componenteCurricularesId, professorCJ }));
            }
        }

        public async Task<IEnumerable<QuantidadeAulasDadasBimestreDto>> ObterAulasDadasPorTurmaEBimestre(string turmaCodigo, long tipoCalendarioId, int[] bimestres)
        {
            var query = @"select sum(a.quantidade) as quantidade, P.bimestre 
                          from aula a 
                         inner join periodo_escolar p on p.tipo_calendario_id = a.tipo_calendario_id
	                                        and a.data_aula between p.periodo_inicio and p.periodo_fim
                         inner join registro_frequencia rf on rf.aula_id = a.id
                          where a.tipo_calendario_id = @tipoCalendarioId
                            and a.turma_id = @turmaCodigo
                            and p.bimestre = any(@bimestres)
                         group by p.bimestre";

            var parametros = new
            {
                turmaCodigo,                
                tipoCalendarioId,
                bimestres
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<QuantidadeAulasDadasBimestreDto>(query, parametros);
            }
        }
    }
}
