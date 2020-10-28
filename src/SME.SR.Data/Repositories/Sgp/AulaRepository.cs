using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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

        public async Task<int> ObterAulasDadas(string codigoTurma, string componenteCurricularCodigo, long tipoCalendarioId, int bimestre)
        {
            var query = @"select sum(a.quantidade) 
                          from aula a 
                         inner join periodo_escolar p on p.tipo_calendario_id = a.tipo_calendario_id
 						                    and a.data_aula between p.periodo_inicio and p.periodo_fim
                         inner join registro_frequencia rf on rf.aula_id = a.id
                          where a.tipo_calendario_id = @tipoCalendarioId
                            and a.turma_id = @codigoTurma 
                            and a.disciplina_id = @disciplinaId 
                            and p.bimestre = @bimestre ";

            var parametros = new { 
                CodigoTurma = codigoTurma, 
                DisciplinaId = componenteCurricularCodigo,
                TipoCalendarioId = tipoCalendarioId,
                Bimestre = bimestre
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<int?>(query, parametros) ?? 0;
            }
        }

        public async Task<AulaPrevista> ObterAulaPrevistaFiltro(long tipoCalendarioId, string turmaId, string disciplinaId)
        {
            var query = @"select * from aula_prevista ap
                         where ap.tipo_calendario_id = @tipoCalendarioId and ap.turma_id = @turmaId and
                               ap.disciplina_id = @disciplinaId;";            

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<AulaPrevista>(query, new { tipoCalendarioId, turmaId, disciplinaId });
            }
        }

        public async Task<bool> VerificaExisteAulaCadastrada(long turmaId, string componenteCurricularId, int bimestre, long tipoCalendarioId)
        {
            var query = @"select distinct 1 from aula a inner join turma on a.turma_id = turma.turma_id 
                inner join periodo_escolar p on p.tipo_calendario_id = a.tipo_calendario_id and a.data_aula between p.periodo_inicio and p.periodo_fim
                where turma.id = @turmaId and a.disciplina_id = @componenteCurricularId and a.tipo_calendario_id = @tipoCalendarioId and p.bimestre = @bimestre;";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
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

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return (await conexao.QueryAsync<int>(query, new { componenteCurricularId, bimestre, tipoCalendarioId })).Any();
            }
        }

       // public Task<int> ObterQuantidadeAulas(long turmaId, string componenteCurricularId, string CodigoRF)
        public async Task<bool> VerificaExisteMaisAulaCadastradaNoDia(long turmaId, string componenteCurricularId, long tipoCalendarioId, int bimestre)
        {
            var query = @"select distinct 1 from aula a 
                          inner join periodo_escolar p on p.tipo_calendario_id = a.tipo_calendario_id and a.data_aula between p.periodo_inicio and p.periodo_fim
                          inner join turma on a.turma_id = turma.turma_id 
                          where turma.id = @turmaId
                            and a.tipo_calendario_id = @tipoCalendarioId
                            and a.disciplina_id = @componenteCurricularId 
                            and p.bimestre = @bimestre 
                         group by a.data_aula, a.criado_rf having sum(a.quantidade) > 1";

            var parametros = new
            {
                componenteCurricularId,
                turmaId,
                tipoCalendarioId,
                bimestre
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
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

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
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

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return (await conexao.QueryAsync<int>(query, new { turmaId, componenteCurricularId, tipoCalendarioId, bimestre })).Any();
            }
        }

        public async Task<IEnumerable<AulasNormaisExcedidoControleGradeSinteticoDto>> ObterAulasExcedidas(long turmaId, string componenteCurricularId, long tipoCalendarioId, int bimestre)
        {
            var query = @"select
	                        TO_CHAR(a.data_aula,'dd/MM/YYYY') as DataAula,
	                        sum(a.quantidade) as QuantidadeAulas,
	                        a.criado_por as Professor
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
                        group by
	                        a.data_aula,
	                        a.criado_por
                       having sum(a.quantidade) >= 3
                        order by
	                        data_aula";
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return (await conexao.QueryAsync<AulasNormaisExcedidoControleGradeSinteticoDto>(query, new { turmaId, componenteCurricularId, tipoCalendarioId, bimestre }));
            }
        }
    }
}
