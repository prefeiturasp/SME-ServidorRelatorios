using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SME.SR.Infra.Dtos.FrequenciaMensal;

namespace SME.SR.Data
{
    public class FrequenciaAlunoRepository : IFrequenciaAlunoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public FrequenciaAlunoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<FrequenciaAluno> ObterPorAlunoDataDisciplina(string codigoAluno, DateTime dataAtual,
            TipoFrequenciaAluno tipoFrequencia, string disciplinaId, string codigoTurma)
        {
            var query = FrequenciaAlunoConsultas.FrequenciaPorAlunoDataDisciplina;

            var parametros = new
            {
                CodigoAluno = codigoAluno,
                DataAtual = dataAtual,
                TipoFrequencia = tipoFrequencia,
                DisciplinaId = disciplinaId,
                codigoTurma
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<FrequenciaAluno>(query, parametros);
            }
        }
        
        public async Task<FrequenciaAluno> ObterPorAlunoTurmasDisciplinasDataAsync(string codigoAluno, TipoFrequenciaAluno tipoFrequencia, string disciplinaId, string turmaCodigo, int bimestre)
        {
            var query = new StringBuilder(@"select id,codigo_aluno codigoaluno, tipo, disciplina_id disciplinaid,periodo_inicio periodoinicio, periodo_fim periodofim, 
                                                   bimestre,total_aulas totalaulas, total_ausencias totalausencias, criado_em criadoem, criado_por criadopor, alterado_em alteradoem, 
                                                   alterado_por alteradopor, criado_rf criadorf, alterado_rf alteradorf, excluido, migrado, total_compensacoes totalcompensacoes, 
                                                   turma_id turmaid, periodo_escolar_id periodoescolarid, total_presencas totalpresencas, total_remotos totalremotos 
	                                        from (select fa.*,
				                                         row_number() over (partition by fa.bimestre, fa.disciplina_id order by fa.id desc) sequencia
          	                                        from frequencia_aluno fa
	                                              where not fa.excluido
                                                    and codigo_aluno = @codigoAluno
	       	                                        and tipo = @tipoFrequencia
	                                                and turma_id = @turmaCodigo
	                                                and disciplina_id = @disciplinaId");

            query.AppendLine(") rf where rf.sequencia = 1");

            if (bimestre > 0)
                query.AppendLine(" and rf.bimestre = @bimestre");

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                var retorno =  await conexao.QueryFirstOrDefaultAsync<FrequenciaAluno>(query.ToString(), new
                {
                    codigoAluno,
                    tipoFrequencia,
                    disciplinaId,
                    turmaCodigo,
                    bimestre,
                });
                return retorno;
            }
        }

        public async Task<double> ObterFrequenciaGlobal(string codigoTurma, string codigoAluno)
        {
            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            var parametros = new {CodigoTurma = codigoTurma, CodigoAluno = codigoAluno};
            return await conexao.QueryFirstOrDefaultAsync<double>(FrequenciaAlunoConsultas.FrequenciaGlobal, parametros);
        }

        public async Task<FrequenciaAluno> ObterPorAlunoBimestreAsync(string codigoAluno, int bimestre, TipoFrequenciaAluno tipoFrequencia, string disciplinaId, string codigoTurma)
        {
            var query = FrequenciaAlunoConsultas.FrequenciaPorAlunoBimestreDisciplina;

            var parametros = new
            {
                CodigoAluno = codigoAluno,
                Bimestre = bimestre,
                TipoFrequencia = tipoFrequencia,
                DisciplinaId = disciplinaId,
                codigoTurma
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<FrequenciaAluno>(query, parametros);
            }
        }

        public async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaPorDisciplinaBimestres(string codigoTurma, string codigoAluno, int? bimestre)
        {
            var query = FrequenciaAlunoConsultas.FrequenciaPorAlunoTurmaBimestre(bimestre);
            var parametros = new {CodigoTurma = codigoTurma, CodigoAluno = codigoAluno, Bimestre = bimestre};

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<FrequenciaAluno>(query, parametros);
            }
        }

        public async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaDisciplinaGlobalPorTurma(string[] turmasCodigo, string[] componentesCurricularesId, long tipoCalendarioId)
        {
            var query = @"select  fa.id Id
                                , fa.codigo_aluno as CodigoAluno
                                , fa.disciplina_id as DisciplinaId
                                , pe.bimestre
                                , fa.total_aulas as TotalAulas
                                , fa.total_ausencias as TotalAusencias
                                , fa.total_compensacoes as TotalCompensacoes
                            from frequencia_aluno fa
                           inner join periodo_escolar pe on pe.id = fa.periodo_escolar_id
                            where not excluido 
                              and fa.tipo = 1
                              and fa.turma_id = ANY(@turmasCodigo)
                              and fa.disciplina_id = ANY(@componentesCurricularesId)
                              and pe.tipo_calendario_id = @tipoCalendarioId ";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<FrequenciaAluno>(query, new {turmasCodigo, componentesCurricularesId, tipoCalendarioId});
            }
        }

        public async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciasPorTurmasAlunos(string[] codigosAluno, int anoLetivo, int modalidade, int semestre, string[] turmaCodigos, string professor = null)
        {
            var query = @$"select * 
	                            from( 
		                            select fa.codigo_aluno CodigoAluno, t.turma_id as TurmaId, t.ano_letivo as AnoTurma, 
		                                    t.modalidade_codigo as ModalidadeTurma, fa.tipo, fa.disciplina_id DisciplinaId, 
		                                    fa.periodo_inicio PeriodoInicio, fa.periodo_fim PeriodoFim, fa.bimestre, 
		                                    fa.total_aulas TotalAulas,
		                                    fa.total_ausencias TotalAusencias, 
		                                    fa.total_compensacoes TotalCompensacoes,
		                                    fa.periodo_escolar_id PeriodoEscolarId,
                                            fa.professor_rf Professor,
		                                    row_number() over (partition by fa.codigo_aluno, fa.disciplina_id, fa.periodo_escolar_id, fa.professor_rf order by fa.id desc) sequencia
		                                     from frequencia_aluno fa 
		                                    inner join turma t on t.turma_id = fa.turma_id
			                                where fa.codigo_aluno = ANY(@codigosAluno)
	                                          and fa.tipo = 1
	                                          and t.ano_letivo = @anoLetivo
	                                          and t.modalidade_codigo = @modalidade
	                                          and t.semestre = @semestre
	                                          {(turmaCodigos?.Length > 0 ? " and t.turma_id = ANY(@turmaCodigos) " : string.Empty)}
                                              {(!string.IsNullOrWhiteSpace(professor) ? " and (fa.professor_rf = @professor or fa.professor_rf is null) " : string.Empty)}
		                            )rf 
	                            where rf.sequencia = 1;";

            var parametros = new { codigosAluno, anoLetivo, modalidade, semestre, turmaCodigos, professor };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<FrequenciaAluno>(query, parametros);
            }
        }

        public async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciasPorTurmasAlunosParaHistoricoEscolar(string[] codigosAluno, int anoLetivo, int modalidade, int semestre)
        {
            var query = new StringBuilder(@$"select fa.codigo_aluno CodigoAluno, t.turma_id TurmaId, t.ano_letivo as AnoTurma, t.modalidade_codigo as ModalidadeTurma,
                            fa.tipo, fa.disciplina_id DisciplinaId, fa.periodo_inicio PeriodoInicio, 
                            fa.periodo_fim PeriodoFim, fa.bimestre, sum(fa.total_aulas) TotalAulas, 
                            sum(fa.total_ausencias) TotalAusencias, sum(fa.total_compensacoes) TotalCompensacoes, 
                            fa.periodo_escolar_id PeriodoEscolarId
                             from frequencia_aluno fa 
                            inner join turma t on t.turma_id = fa.turma_id
                            where fa.codigo_aluno = ANY(@codigosAluno)
                              and fa.tipo = 1
                              and t.ano_letivo <= @anoLetivo ");

            if (modalidade > 0)
            {
                query.AppendLine(" and t.modalidade_codigo = @modalidade ");
            }

            if (semestre > 0)
            {
                query.AppendLine(" and t.semestre = @semestre ");
            }

            query.AppendLine(@" group by fa.codigo_aluno, fa.tipo, fa.disciplina_id, fa.periodo_inicio, 
                                fa.periodo_fim, fa.bimestre, fa.periodo_escolar_id, t.turma_id, t.ano_letivo, t.modalidade_codigo");

            var parametros = new {codigosAluno, anoLetivo, modalidade, semestre};

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<FrequenciaAluno>(query.ToString(), parametros);
            }
        }

        public async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaGlobalAlunos(string[] codigosAluno, int anoLetivo, int modalidade, string[] codigoTurmas)
        {
            var query = @$"with frequenciaGeral as (select fa.codigo_aluno as CodigoAluno
                                , sum(fa.total_aulas) as TotalAulas
                                , sum(fa.total_ausencias) as TotalAusencias
                                , sum(fa.total_compensacoes) as TotalCompensacoes
                                , row_number() over (partition by fa.codigo_aluno, fa.bimestre order by fa.id) as sequencia
                              from frequencia_aluno fa 
                            inner join turma t on t.turma_id = fa.turma_id
                            where fa.codigo_aluno = any(@codigosAluno) 
                              and t.ano_letivo = @anoLetivo
                              and t.modalidade_codigo = @modalidade 
                              and fa.tipo = 2
                              and t.tipo_turma in(1,2,7) 
                              and fa.turma_id = any(@codigoTurmas)
                            group by fa.codigo_aluno, fa.bimestre, fa.id)
                            select codigoAluno as CodigoAluno, 
                            sum(totalaulas) as TotalAulas,
                            sum(totalausencias) as TotalAusencias,
                            sum(totalCompensacoes) as TotalCompensacoes
                            from frequenciaGeral where sequencia = 1
                            group by codigoAluno
                            ";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<FrequenciaAluno>(query, new { codigosAluno, anoLetivo, modalidade, codigoTurmas });
            }
        }

        public async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaPorComponentesBimestresTurmas(string[] componentesCurriculares, int[] bimestres, string[] turmasCodigos)
        {
            var query = new StringBuilder(@"select freq.Id as Id, freq.codigoAluno as CodigoAluno, freq.bimestre as Bimestre, 
                                            freq.TurmaId as TurmaId, freq.disciplinaId as DisciplinaId, freq.periodoEscolarId as PeriodoEscolarId,
                                            freq.periodoFim as PeriodoFim, freq.PeriodoInicio as PeriodoInicio, freq.totalAulas as TotalAulas,
                                            freq.totalCompensacoes as TotalCompensacoes, freq.Tipo as Tipo, freq.totalAusencias as TotalAusencias
                                            from 
                                            (select fa.id Id, fa.codigo_aluno as codigoAluno, 
                                            fa.bimestre, fa.turma_id as TurmaId, fa.disciplina_id as disciplinaId,
                                            fa.periodo_escolar_id as periodoEscolarId, fa.periodo_fim as PeriodoFim, fa.periodo_inicio as PeriodoInicio,
                                            fa.total_aulas as totalAulas, fa.total_compensacoes as totalCompensacoes, fa.tipo as Tipo, fa.total_ausencias as totalAusencias,
                                            row_number() over (partition by fa.codigo_aluno, fa.turma_id, fa.bimestre, fa.disciplina_id order by fa.id desc) as sequencia
                                            from frequencia_aluno fa 
                                            where 1=1 and fa.total_aulas <> 0 ");

            if (componentesCurriculares.Any())
                query.AppendLine("and disciplina_id = any(@componentesCurriculares)");

            if (bimestres.Any())
                query.AppendLine("and bimestre = any(@bimestres)");

            if (turmasCodigos.Any())
                query.AppendLine("and turma_id = any(@turmasCodigos)");

            query.AppendLine(" ) freq where freq.sequencia = 1");

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<FrequenciaAluno>(query.ToString(), new {componentesCurriculares, bimestres, turmasCodigos});
            }
        }

        public async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaGeralAlunoPorAnoModalidadeSemestre(int anoTurma, long tipoCalendarioId)
        {
            var query = new StringBuilder($@"select fa.id Id
                                , fa.codigo_aluno as CodigoAluno
                                , fa.turma_id as TurmaId
                                , fa.total_aulas as TotalAulas
                                , fa.total_ausencias as TotalAusencias
                                , fa.total_compensacoes as TotalCompensacoes
                                {(tipoCalendarioId > 0 ? ", pe.bimestre Bimestre" : string.Empty)}
                            from frequencia_aluno fa
                            inner join turma t on fa.turma_id = t.turma_id ");

            if (tipoCalendarioId > 0)
                query.AppendLine("inner join periodo_escolar pe on fa.periodo_escolar_id = pe.id");

            query.AppendLine(@" where fa.tipo = 2 
                and t.ano_letivo = @anoTurma 
                and t.tipo_turma in(1,2,7) ");

            if (tipoCalendarioId > 0)
                query.AppendLine(" and pe.tipo_calendario_id = @tipoCalendarioId");

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao
                    .QueryAsync<FrequenciaAluno>(query.ToString(), new
                    {
                        anoTurma,
                        tipoCalendarioId
                    });
            }
        }

        public async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaGeralPorAnoModalidadeSemestreEAlunos(int anoTurma, long tipoCalendarioId, string[] alunosCodigo, string turmaCodigo)
        {
            var query = new StringBuilder($@"with lista as (
                           select fa.id Id
                                , fa.codigo_aluno as CodigoAluno
                                , fa.turma_id as TurmaId
                                , fa.total_aulas as TotalAulas
                                , fa.total_ausencias as TotalAusencias
                                , fa.total_compensacoes as TotalCompensacoes
                                {(tipoCalendarioId > 0 ? ", pe.bimestre Bimestre" : string.Empty)}
                                , row_number() over (partition by fa.codigo_aluno, fa.periodo_escolar_id, fa.tipo, fa.disciplina_id order by fa.id desc) sequencia
                            from frequencia_aluno fa
                            inner join turma t on fa.turma_id = t.turma_id ");

            if (tipoCalendarioId > 0)
                query.AppendLine("inner join periodo_escolar pe on fa.periodo_escolar_id = pe.id");

            query.AppendLine(@" where fa.tipo = 2 
                                      and t.ano_letivo = @anoTurma 
                                      and fa.codigo_aluno = any(@alunosCodigo)
                                      and t.tipo_turma in(1,2,7) 
                                      and t.turma_id = @turmaCodigo");

            if (tipoCalendarioId > 0)
                query.AppendLine(" and pe.tipo_calendario_id = @tipoCalendarioId");

            query.AppendLine(")");
            query.AppendLine("select * from lista where sequencia = 1;");

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao
                    .QueryAsync<FrequenciaAluno>(query.ToString(), new
                    {
                        anoTurma,
                        tipoCalendarioId,
                        alunosCodigo,
                        turmaCodigo
                    });
            }
        }

        public async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaGeralAlunosPorTurmaEBimestre(long turmaId, string alunoCodigo, int[] bimestres)
        {
            var query = new StringBuilder(@$"select * from (select fa.id Id
                                                    , fa.codigo_aluno as CodigoAluno
                                                    , fa.turma_id as TurmaId
                                                    , fa.total_aulas as TotalAulas
                                                    , fa.total_ausencias as TotalAusencias
                                                    , fa.total_compensacoes as TotalCompensacoes
                                                    , fa.bimestre
                                                    , row_number() over (partition by fa.turma_id, fa.codigo_aluno, fa.bimestre, fa.disciplina_id, fa.tipo order by fa.id desc) sequencia
                                              from frequencia_aluno fa
                                             inner join turma t on fa.turma_id = t.turma_id
                                             inner join periodo_escolar pe on fa.periodo_escolar_id = pe.id
                                             where fa.tipo = 2
                                               and t.id = @turmaId
                                               and pe.bimestre = any(@bimestres)");

            if (!string.IsNullOrEmpty(alunoCodigo))
                query.AppendLine("and fa.codigo_aluno = @alunoCodigo");

            query.AppendLine(") as freqAluno where freqAluno.sequencia = 1");

            var parametros = new {turmaId, alunoCodigo, bimestres};

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<FrequenciaAluno>(query.ToString(), parametros);
            }
        }

        public async Task<DateTime?> ObterUltimaFrequenciaRegistradaProfessor(string professorRf)
        {
            var query = @$"select max(rf.criado_em) 
                          from aula a
                         inner join registro_frequencia rf on rf.aula_id = a.id
                         where not a.excluido 
                           and a.professor_rf = @professorRf";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<DateTime?>(query, new {professorRf});
            }
        }

        public async Task<bool> ExisteFrequenciaRegistradaPorTurmaComponenteCurricular(string codigoTurma, string componenteCurricularId, long periodoEscolarId, int[] bimestres)
        {
            var query = @"select distinct(1)
                            from registro_frequencia_aluno rfa
                           inner join registro_frequencia rf on rf.id = rfa.registro_frequencia_id 
                           inner join aula a on a.id = rf.aula_id 
                           inner join tipo_calendario tc on tc.id = a.tipo_calendario_id
                           inner join periodo_escolar pe on pe.tipo_calendario_id = tc.id
                           where pe.id = @periodoEscolarId
                             and a.turma_id = @codigoTurma ";
            if (!String.IsNullOrEmpty(componenteCurricularId) && componenteCurricularId != "0")
                query += @" and a.disciplina_id = @componenteCurricularId ";

            query += @" and a.data_aula between pe.periodo_inicio and pe.periodo_fim ";

            if (bimestres.Count() > 0)
                query += @" and pe.bimestre = ANY(@bimestres) ";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryFirstOrDefaultAsync<bool>(query, new {codigoTurma, componenteCurricularId, periodoEscolarId, bimestres});
        }

        public async Task<bool> ExisteFrequenciaRegistradaPorTurmaComponenteCurricularEAno(string codigoTurma, string componenteCurricularId, int anoLetivo)
        {
            var query = @"select distinct(1)
                            from registro_frequencia_aluno rfa
                           inner join registro_frequencia rf on rf.id = rfa.registro_frequencia_id 
                           inner join aula a on a.id = rf.aula_id
                           where a.turma_id = @codigoTurma
                             and a.disciplina_id = @componenteCurricularId
                             and extract(year from a.data_aula) = @anoLetivo ";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            var resultado = await conexao
                .QueryFirstOrDefaultAsync<int>(query, new { codigoTurma, componenteCurricularId, anoLetivo });

            return resultado == 1;
        }

        public async Task<IEnumerable<FrequenciaAlunoRetornoDto>> ObterFrequenciasAlunosPorTurmas(string[] codigosturma)
        {
            var query = @"select t.turma_id as TurmaCodigo,
                                 rfa.codigo_aluno as AlunoCodigo,
	                             rfa.valor as TipoFrequencia,	   
                                 sum(rfa.numero_aula) as Quantidade       
                            from registro_frequencia_aluno rfa 
                           inner join registro_frequencia rf on rf.id = rfa.registro_frequencia_id 
                           inner join aula a on a.id = rf.aula_id 
                           inner join turma t on t.turma_id = a.turma_id 
                           inner join ue on ue.id = t.ue_id 
                           inner join dre on dre.id = ue.dre_id 
                           where not rfa.excluido 
                             and t.turma_id = Any(@codigosturma)
                           group by t.turma_id, rfa.codigo_aluno, rfa.valor";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<FrequenciaAlunoRetornoDto>(query, new {codigosturma});
            }
        }

        public async Task<IEnumerable<FrequenciaAlunoRetornoDto>> ObterFrequenciasAlunosPorFiltro(string[] codigosturma, string componenteCurricularId, int bimestre)
        {
            var query = @"select t.turma_id as TurmaCodigo,
                            rfa.codigo_aluno as AlunoCodigo,
                            rfa.valor as TipoFrequencia,	   
                            count(rfa.numero_aula) as Quantidade
                          from registro_frequencia_aluno rfa 
                          inner join registro_frequencia rf on rf.id = rfa.registro_frequencia_id and not rfa.excluido and not rf.excluido
                          inner join aula a on a.id = rf.aula_id
                          inner join periodo_escolar pe on a.tipo_calendario_id = pe.tipo_calendario_id 
                          and a.data_aula >= pe.periodo_inicio and a.data_aula <= pe.periodo_fim
                          inner join turma t on t.turma_id = a.turma_id 
                          inner join ue on ue.id = t.ue_id 
                          inner join dre on dre.id = ue.dre_id 
                            where not rfa.excluido 
                                and t.turma_id = Any(@codigosturma)
                                and pe.bimestre = @bimestre
                                and a.disciplina_id = @componenteCurricularId
                            group by t.turma_id, rfa.codigo_aluno, rfa.valor";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<FrequenciaAlunoRetornoDto>(query, new {codigosturma, componenteCurricularId, bimestre});
            }
        }

        public async Task<IEnumerable<FrequenciaAluno>> ObterRegistroFrequenciasPorTurmasAlunos(string[] codigosAluno, int anoLetivo, int modalidade, int semestre)
        {
            var query = @$"select fa.codigo_aluno CodigoAluno, t.turma_id as TurmaId, t.ano_letivo as AnoTurma, 
                            t.modalidade_codigo as ModalidadeTurma, fa.tipo, fa.disciplina_id DisciplinaId, 
                            fa.periodo_inicio PeriodoInicio, fa.periodo_fim PeriodoFim, fa.bimestre, 
                            sum(fa.total_aulas) TotalAulas, sum(fa.total_ausencias) TotalAusencias, 
                            sum(fa.total_compensacoes) TotalCompensacoes, fa.periodo_escolar_id PeriodoEscolarId
                             from frequencia_aluno fa 
                            inner join turma t on t.turma_id = fa.turma_id
                            where fa.codigo_aluno = ANY(@codigosAluno)
                              and fa.tipo = 1
                              and t.ano_letivo = @anoLetivo
                              and t.modalidade_codigo = @modalidade
                              and t.semestre = @semestre
                            group by fa.codigo_aluno, fa.tipo, fa.disciplina_id, fa.periodo_inicio, 
                            fa.periodo_fim, fa.bimestre, fa.periodo_escolar_id, t.ano_letivo, t.modalidade_codigo, t.turma_id";

            var parametros = new {codigosAluno, anoLetivo, modalidade, semestre};

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<FrequenciaAluno>(query, parametros);
            }
        }

        public async Task<IEnumerable<string>> ObterAlunosComRegistroFrequenciaPorTurmaBimestre(string turmaCodigo, int bimestre)
        {
            var query = @"select distinct rfa.codigo_aluno as AlunoCodigo
                          from aula a
                         inner join registro_frequencia rf on 
 	                        rf.aula_id = a.id
 	                        and not rf.excluido 
                         inner join registro_frequencia_aluno rfa on 
 	                        rfa.registro_frequencia_id = rf.id
 	                        and not rfa.excluido 
                         inner join periodo_escolar pe on 
 	                        pe.tipo_calendario_id = a.tipo_calendario_id 
	                        and a.data_aula between pe.periodo_inicio and pe.periodo_fim 
                         where not a.excluido 
                           and a.turma_id = @turmaCodigo
                           and pe.bimestre = @bimestre";
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<string>(query, new {turmaCodigo, bimestre});
            }
        }

        public async Task<IEnumerable<FrequenciaAlunoConsolidadoDto>> ObterFrequenciaAlunosPorCodigoBimestre(string[] codigosAlunos, string bimestre, string turmaCodigo, TipoFrequenciaAluno tipoFrequencia, string[] componentesCurricularesIds)
        {
            var query = new StringBuilder(@"select extract(year from periodo_inicio) as AnoBimestre,
                                                faa.bimestre,
	                                            sum(faa.total_aulas) as TotalAula,
	                                            sum(faa.total_presencas) as TotalPresencas,
	                                            sum(faa.total_remotos) as TotalRemotos,
	                                            sum(faa.total_ausencias) as TotalAusencias,
	                                            sum(faa.total_compensacoes) as TotalCompensacoes,
	                                            faa.codigo_aluno as CodigoAluno from (
                                                        select fa.*,  row_number() over (partition by turma_id, codigo_aluno, bimestre, disciplina_id, tipo order by id desc) sequencia
                                                        from frequencia_aluno fa
                                                        where not fa.excluido 
                                                        and fa.tipo = @tipoFrequencia
                                                        and fa.codigo_aluno = any(@codigosAlunos)  
                                                        and fa.turma_id = @turmaCodigo ");

            if (componentesCurricularesIds.Any() && componentesCurricularesIds != null)
                query.AppendLine(" and fa.disciplina_id = any(@componentesCurricularesIds)");

            if (bimestre != "-99")
                query.AppendLine("and fa.bimestre = @numeroBimestre ");

            query.AppendLine(@" ) as faa where faa.sequencia = 1");
            query.AppendLine(@" group by 
                                faa.bimestre,
                                faa.codigo_aluno,faa.periodo_inicio;");
            var parametros = new { codigosAlunos, numeroBimestre = Convert.ToInt32(bimestre), turmaCodigo, tipoFrequencia, componentesCurricularesIds };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<FrequenciaAlunoConsolidadoDto>(query.ToString(), parametros);
            }
        }

        public async Task<IEnumerable<AusenciaBimestreDto>> ObterAusenciaPorAlunoTurmaBimestre(string[] alunosCodigo, string turmaCodigo, string bimestre, string[] componentesCurricularesIds = null)
        {
            var query = @" select
 	                         afa.codigo_aluno as codigoAluno,
                             a.data_aula dataAusencia,
                             CONCAT(ma.descricao, ' ', afa.anotacao) as motivoAusencia, 
                             pe.bimestre
                         from 
                             anotacao_frequencia_aluno afa 
                         inner join aula a on a.id = afa.aula_id 
                         inner join tipo_calendario tc on tc.id = a.tipo_calendario_id 
                         inner join periodo_escolar pe on pe.tipo_calendario_id = tc.id
                          left join motivo_ausencia ma on afa.motivo_ausencia_id = ma.id 
                         where 
                             not afa.excluido and not a.excluido 
                             and afa.codigo_aluno = any(@alunosCodigo)	 
                             and a.turma_id = @turmaCodigo";

            if (bimestre != "-99")
                query += " and pe.bimestre = @numeroBimestre ";

            if (componentesCurricularesIds != null && componentesCurricularesIds.Any())
                query += " and a.disciplina_id = any(@componentesCurricularesIds) ";

            query += @" and a.data_aula between pe.periodo_inicio and pe.periodo_fim 
                order by pe.bimestre,a.data_aula desc; ";

            var parametros = new
            {
                alunosCodigo,
                turmaCodigo,
                numeroBimestre = Convert.ToInt32(bimestre),
                componentesCurricularesIds
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<AusenciaBimestreDto>(query, parametros);
            }
        }

        public async Task<IEnumerable<FrequenciaAlunoConsolidadoRelatorioDto>> ObterFrequenciaAlunosRelatorio(string[] turmasCodigo, string bimestre, string componenteCurricularId)
        {
            var query = new StringBuilder(@"select fa.turma_id TurmaCodigo,
    	                                    fa.bimestre,
    	                                    fa.disciplina_id ComponenteCurricularId,
                                            fa.codigo_aluno AlunoCodigo,
                                            fa.total_aulas TotalAulas,
                                            fa.total_presencas TotalPresencas,
                                            fa.total_remotos TotalRemotos, 
                                            fa.total_ausencias TotalAusencias,
                                            fa.total_compensacoes  Totalcompensacoes
                                       from frequencia_aluno fa
                                      where fa.tipo = @tipoFrequencia
                                        and fa.turma_id = any(@turmasCodigo)");
            if (!string.IsNullOrEmpty(componenteCurricularId))
                query.AppendLine(" and fa.disciplina_id = @componenteCurricularId");

            if (bimestre != "-99")
                query.AppendLine(" and fa.bimestre = @bimestre ");

            var parametros = new {turmasCodigo, bimestre = int.Parse(bimestre), componenteCurricularId, tipoFrequencia = (int) TipoFrequenciaAluno.PorDisciplina};

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<FrequenciaAlunoConsolidadoRelatorioDto>(query.ToString(), parametros);
            }
        }

        public async Task<IEnumerable<FrequenciaAlunoMensalConsolidadoDto>> ObterFrequenciaAlunoMensal(bool exibirHistorico, int anoLetivo, string codigoDre,
            string codigoUe, Modalidade modalidade, int semestre, string[] codigosTurmas, int[] mesesReferencias, int percentualAbaixoDe)
        {
            var query = @"select distinct d.dre_id AS DreCodigo, d.abreviacao as DreSigla,
                                u.nome as UeNome,
                                u.ue_id AS UeCodigo,
                                te.descricao as DescricaoTipoEscola,
                                cfam.mes,
                                t.modalidade_codigo as ModalidadeCodigo,
                                t.turma_id AS TurmaCodigo,
                                t.nome as TurmaNome,
                                cfam.aluno_codigo as CodigoEol,
                                cfam.percentual
                            from consolidacao_frequencia_aluno_mensal cfam
                                inner join turma t on t.id = cfam.turma_id
                                inner join ue u on u.id = t.ue_id
                                inner join tipo_escola te on te.cod_tipo_escola_eol = u.tipo_escola
                                inner join dre d on d.id = u.dre_id
                            where t.ano_letivo = @anoLetivo
                            and t.modalidade_codigo = @modalidade";

            if (codigoDre != "-99")
                query += " and d.dre_id = @codigoDre";

            if (codigoUe != "-99")
                query += " and u.ue_id = @codigoUe";

            if (!exibirHistorico)
                query += " and not t.historica ";

            if (semestre > 0)
                query += " and t.semestre = @semestre ";

            if (codigosTurmas.Length > 0 && !codigosTurmas.Contains("-99"))
                query += " and t.turma_id = any(@codigosTurmas) ";

            if (mesesReferencias.Length > 0 && !mesesReferencias.Contains(-99))
                query += " and cfam.mes = any(@mesesReferencias) ";
            else
                query += " and cfam.mes > 1";

            if (percentualAbaixoDe > 0)
                query += " and cfam.percentual < @percentualAbaixoDe";

            var parametros = new
            {
                exibirHistorico,
                anoLetivo,
                codigoDre,
                codigoUe,
                modalidade,
                semestre,
                codigosTurmas,
                mesesReferencias,
                percentualAbaixoDe
            };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryAsync<FrequenciaAlunoMensalConsolidadoDto>(query, parametros);
        }

        public async Task<IEnumerable<RelatorioFrequenciaIndividualDiariaAlunoDto>> ObterFrequenciaAlunosDiario(
                                                                    string[] codigosAlunos, 
                                                                    string bimestre, 
                                                                    string turmaCodigo, 
                                                                    string[] componentesCurricularesIds,
                                                                    string professorTitularRf = null)
        {
            var condicaoBimestre = string.Empty;

            if (bimestre != "-99")
                condicaoBimestre = " AND pe.bimestre = @bimestre";

            var query = @$"SELECT count(rfa.id) AS QuantidadeAulas,
	                        a.data_aula AS DataAula,
	                        a.id AS AulasId,
	                        rfa.codigo_aluno AS AlunoCodigo,
	                        an.id AS AnotacaoId,
                            count(distinct(rfa.registro_frequencia_id*rfa.numero_aula)) filter (WHERE rfa.valor = 1) AS QuantidadePresenca,
                            count(distinct(rfa.registro_frequencia_id*rfa.numero_aula)) filter (WHERE rfa.valor = 2) AS QuantidadeAusencia,
                            count(distinct(rfa.registro_frequencia_id*rfa.numero_aula)) filter (WHERE rfa.valor = 3) AS QuantidadeRemoto,
                            coalesce(ma.descricao, an.anotacao) as Motivo,
                            pe.bimestre AS Bimestre
                        FROM registro_frequencia_aluno rfa 
                        INNER JOIN registro_frequencia rf ON rfa.registro_frequencia_id = rf.id
                        INNER JOIN aula a ON rf.aula_id = a.id
                        INNER JOIN turma t ON t.turma_id = a.turma_id
                        INNER JOIN periodo_escolar pe ON a.tipo_calendario_id = pe.tipo_calendario_id AND a.data_aula BETWEEN pe.periodo_inicio AND pe.periodo_fim {condicaoBimestre}
                        LEFT JOIN anotacao_frequencia_aluno an ON a.id = an.aula_id AND an.codigo_aluno  = rfa.codigo_aluno AND an.excluido = false
                        LEFT JOIN motivo_ausencia ma ON an.motivo_ausencia_id = ma.id
                        WHERE NOT rfa.excluido 
                            AND NOT rf.excluido 
                            AND NOT a.excluido
	                        AND rfa.codigo_aluno = any(@codigosAlunos)
	                        AND t.turma_id = @turmaCodigo 
                            AND a.disciplina_id = any(@componentesCurricularesIds)
                           {(!string.IsNullOrEmpty(professorTitularRf) ? " and a.professor_rf = @professorTitularRf" : string.Empty)}
 						GROUP BY a.data_aula, a.id, an.id, ma.descricao, rfa.codigo_aluno, pe.bimestre
                        ORDER BY pe.bimestre, rfa.codigo_aluno, a.data_aula desc";

            var parametros = new
            {
                codigosAlunos,
                bimestre = int.Parse(bimestre),
                turmaCodigo,
                componentesCurricularesIds,
                professorTitularRf
            };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryAsync<RelatorioFrequenciaIndividualDiariaAlunoDto>(query, parametros);
        }

        public async Task<IEnumerable<ConsultaRelatorioFrequenciaControleMensalDto>> ObterFrequenciaControleMensal(int anoLetivo, string[] mes, string ueCodigo, string dreCodigo, int modalidade,
            int semestre, string turmaCodigo, string[] alunosCodigo)
        {
            var bimestres = semestre <= 2 ? new int[] {1, 2} : new int[] {3, 4};
            var todosAlunos = !alunosCodigo.Any();
            var todosMeses = mes.Contains("-99");
            var sql = new StringBuilder();
            sql.AppendLine(@"with controle as(");
            sql.AppendLine(@"SELECT ccgm.nome as NomeGrupo,");
            sql.AppendLine(@"       a.disciplina_id as disciplinaid,");
            sql.AppendLine(@"       cc.descricao_sgp as NomeComponente,");
            sql.AppendLine(@"       rfa.codigo_aluno as CodigoAluno, ");
            sql.AppendLine(@"       cc.descricao_sgp as NomeComponente, ");
            sql.AppendLine(@"       a.quantidade TotalAula,");
            sql.AppendLine(@"       count(valor) AS TotalTipoFrequencia,");
            sql.AppendLine(@"       valor as TipoFrequencia,");
            sql.AppendLine(@"       a.data_aula as DataAula,");
            sql.AppendLine(@"       extract('month' from a.data_aula) as Mes,");
            sql.AppendLine(@"       extract('day' from a.data_aula) as Dia,");
            sql.AppendLine(@"       case ");
            sql.AppendLine(@"         when extract(dow from a.data_aula)=0 THEN 'DOM'");
            sql.AppendLine(@"         when extract(dow from a.data_aula)=1 THEN 'SEG'");
            sql.AppendLine(@"         when extract(dow from a.data_aula)=2 THEN 'TER'");
            sql.AppendLine(@"         when extract(dow from a.data_aula)=3 THEN 'QUA'");
            sql.AppendLine(@"         when extract(dow from a.data_aula)=4 THEN 'QUI'");
            sql.AppendLine(@"         when extract(dow from a.data_aula)=5 THEN 'SEX'");
            sql.AppendLine(@"         when extract(dow from a.data_aula)=6 THEN 'SAB'");
            sql.AppendLine(@"       end DiaSemana,");
            sql.AppendLine(@"       TotalCompensacao,");
            sql.AppendLine(@"       compensacao.data_aula as DataCompensacao, ");
            sql.AppendLine(@"       pe.bimestre ");
            sql.AppendLine(@"FROM registro_frequencia_aluno rfa");
            sql.AppendLine(@"INNER JOIN registro_frequencia rf ON rfa.registro_frequencia_id = rf.id");
            sql.AppendLine(@"INNER JOIN aula a ON rfa.aula_id = a.id");
            sql.AppendLine(@" left join componente_curricular cc on cc.id = a.disciplina_id::int8");
            sql.AppendLine(@" left join componente_curricular_grupo_matriz ccgm on cc.grupo_matriz_id = ccgm.id ");
            sql.AppendLine(@" left join componente_curricular_area_conhecimento ccac on cc.area_conhecimento_id = ccac.id");
            sql.AppendLine(@" inner join ue u on a.ue_id = u.ue_id  ");
            sql.AppendLine(@"  inner join dre d on u.dre_id = d.id  ");
            sql.AppendLine(@"inner join tipo_calendario tc on a.tipo_calendario_id = tc.id ");
            sql.AppendLine(@"inner join periodo_escolar pe on tc.periodo = pe.id ");
            sql.AppendLine(@"INNER JOIN turma t ON t.turma_id  = a.turma_id");
            sql.AppendLine(@"LEFT JOIN");
            sql.AppendLine(@"  (SELECT DISTINCT  rfa.codigo_aluno, a.id idAula,");
            sql.AppendLine(@"                   a.data_aula,");
            sql.AppendLine(@"                   a.disciplina_id,");
            sql.AppendLine(@"                   count(caaa.numero_aula) totalCompensacao");
            sql.AppendLine(@"   FROM compensacao_ausencia_aluno_aula caaa");
            sql.AppendLine(@"   INNER JOIN registro_frequencia_aluno rfa ON rfa.id = caaa.registro_frequencia_aluno_id");
            sql.AppendLine(@"   INNER JOIN registro_frequencia rf ON rfa.registro_frequencia_id = rf.id");
            sql.AppendLine(@"   INNER JOIN aula a ON rfa.aula_id = a.id");
            sql.AppendLine(@"   WHERE NOT caaa.excluido AND NOT rfa.excluido");
            sql.AppendLine(@"     AND NOT rf.excluido AND NOT a.excluido");
            if (!todosAlunos)
                sql.AppendLine(@"     and rfa.codigo_aluno = any(@alunosCodigo)");
            if (!todosMeses)
                sql.AppendLine(@"     AND extract(month FROM a.data_aula)::text   = any(@mes) ");
            sql.AppendLine(@"     AND a.turma_id  = @turmaCodigo and a.ue_id = @ueCodigo  ");

            sql.AppendLine(@"   GROUP BY  rfa.codigo_aluno, a.disciplina_id,");
            sql.AppendLine(@"            a.data_aula,");
            sql.AppendLine(@"            a.id) compensacao ON compensacao.idAula = a.id and compensacao.codigo_aluno = rfa.codigo_aluno ");
            sql.AppendLine(@"WHERE NOT rfa.excluido AND NOT rf.excluido  AND NOT a.excluido");
            if (!todosAlunos)
                sql.AppendLine(@"  and rfa.codigo_aluno= any(@alunosCodigo) ");
            if (semestre > 0)
                sql.AppendLine(@"  and pe.bimestre = any(@bimestres)");
            sql.AppendLine(@"  and u.ue_id  = @ueCodigo ");
            sql.AppendLine(@"  and d.dre_id  = @dreCodigo ");
            sql.AppendLine(@"  and t.ano_letivo  = @anoLetivo ");
            sql.AppendLine(@"  and t.modalidade_codigo = @modalidade ");
            if (!todosMeses)
                sql.AppendLine(@"  AND extract(month FROM a.data_aula)::text    = any(@mes) ");
            sql.AppendLine(@"  AND a.turma_id = @turmaCodigo ");
            sql.AppendLine(@"GROUP BY ccgm.nome,");
            sql.AppendLine(@"		 a.disciplina_id,");
            sql.AppendLine(@"         rfa.codigo_aluno, ");
            sql.AppendLine(@"         cc.descricao_sgp,");
            sql.AppendLine(@"         a.quantidade,");
            sql.AppendLine(@"         rfa.valor,");
            sql.AppendLine(@"         a.data_aula,");
            sql.AppendLine(@"         totalCompensacao,");
            sql.AppendLine(@"         compensacao.data_aula, ");
            sql.AppendLine(@"         pe.bimestre ");
            sql.AppendLine(@"ORDER BY ccgm.nome,cc.descricao_sgp)");
            sql.AppendLine(@" select * from controle ");

            var parametros = new
            {
                anoLetivo,
                mes,
                ueCodigo,
                dreCodigo,
                modalidade,
                bimestres,
                turmaCodigo,
                alunosCodigo
            };
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                var resultadoConsulta  = await conexao.QueryAsync<ConsultaRelatorioFrequenciaControleMensalDto>(sql.ToString(), parametros);
                return resultadoConsulta;
            }
        }
    }
}