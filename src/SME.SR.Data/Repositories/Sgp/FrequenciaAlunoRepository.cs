using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<double> ObterFrequenciaGlobal(string codigoTurma, string codigoAluno)
        {
            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            var parametros = new { CodigoTurma = codigoTurma, CodigoAluno = codigoAluno };
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
            var parametros = new { CodigoTurma = codigoTurma, CodigoAluno = codigoAluno, Bimestre = bimestre };

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
                return await conexao.QueryAsync<FrequenciaAluno>(query, new { turmasCodigo, componentesCurricularesId, tipoCalendarioId });
            }
        }

        public async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciasPorTurmasAlunos(string[] codigosAluno, int anoLetivo, int modalidade, int semestre)
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

            var parametros = new { codigosAluno, anoLetivo, modalidade, semestre };

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

            var parametros = new { codigosAluno, anoLetivo, modalidade, semestre };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<FrequenciaAluno>(query.ToString(), parametros);
            }
        }

        public async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaGlobalAlunos(string[] codigosAluno, int anoLetivo, int modalidade)
        {
            var query = @$"select fa.codigo_aluno as CodigoAluno
                                , sum(fa.total_aulas) as TotalAulas
                                , sum(fa.total_ausencias) as TotalAusencias
                                , sum(fa.total_compensacoes) as TotalCompensacoes
                              from frequencia_aluno fa 
                            inner join turma t on t.turma_id = fa.turma_id
                            where fa.codigo_aluno = ANY(@codigosAluno) 
                              and t.ano_letivo = @anoLetivo
                              and t.modalidade_codigo = @modalidade 
                              and fa.tipo = 2
                              and t.tipo_turma in(1,2,7) 
                            group by fa.codigo_aluno";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<FrequenciaAluno>(query, new { codigosAluno, anoLetivo, modalidade });
            }
        }

        public async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaPorComponentesBimestresTurmas(string[] componentesCurriculares, int[] bimestres, string[] turmasCodigos)
        {
            var query = new StringBuilder(@"select fa.id Id, fa.codigo_aluno as codigoAluno, fa.bimestre, fa.turma_id as TurmaId, fa.disciplina_id as disciplinaId,
                                         fa.periodo_escolar_id as periodoEscolarId, fa.periodo_fim as PeriodoFim, fa.periodo_inicio as PeriodoInicio,
                                         fa.total_aulas as totalAulas, fa.total_compensacoes as totalCompensacoes, fa.tipo, fa.total_ausencias as totalAusencias
                                         from frequencia_aluno fa 
                                        where 1=1 ");

            if (componentesCurriculares.Any())
                query.AppendLine("and disciplina_id = any(@componentesCurriculares)");

            if (bimestres.Any())
                query.AppendLine("and bimestre = any(@bimestres)");

            if (turmasCodigos.Any())
                query.AppendLine("and turma_id = any(@turmasCodigos)");

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<FrequenciaAluno>(query.ToString(), new { componentesCurriculares, bimestres, turmasCodigos });
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

        public async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaGeralPorAnoModalidadeSemestreEAlunos(int anoTurma, long tipoCalendarioId, string[] alunosCodigo)
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
                                      and fa.codigo_aluno = any(@alunosCodigo)
                                      and t.tipo_turma in(1,2,7) ");

            if (tipoCalendarioId > 0)
                query.AppendLine(" and pe.tipo_calendario_id = @tipoCalendarioId");

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao
                .QueryAsync<FrequenciaAluno>(query.ToString(), new
                {
                    anoTurma,
                    tipoCalendarioId,
                    alunosCodigo
                });
            }
        }

        public async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaGeralAlunosPorTurmaEBimestre(long turmaId, string alunoCodigo, int[] bimestres)
        {
            var query = new StringBuilder(@$"select fa.id Id
                                                    , fa.codigo_aluno as CodigoAluno
                                                    , fa.turma_id as TurmaId
                                                    , fa.total_aulas as TotalAulas
                                                    , fa.total_ausencias as TotalAusencias
                                                    , fa.total_compensacoes as TotalCompensacoes
                                                    , fa.bimestre
                                              from frequencia_aluno fa
                                             inner join turma t on fa.turma_id = t.turma_id
                                             inner join periodo_escolar pe on fa.periodo_escolar_id = pe.id
                                             where fa.tipo = 2
                                               and t.id = @turmaId
                                               and pe.bimestre = any(@bimestres)");

            if (!string.IsNullOrEmpty(alunoCodigo))
                query.AppendLine("and fa.codigo_aluno = @alunoCodigo");

            var parametros = new { turmaId, alunoCodigo, bimestres };

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
                return await conexao.QueryFirstOrDefaultAsync<DateTime?>(query, new { professorRf });
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

            return await conexao.QueryFirstOrDefaultAsync<bool>(query, new { codigoTurma, componenteCurricularId, periodoEscolarId, bimestres });
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
                return await conexao.QueryAsync<FrequenciaAlunoRetornoDto>(query, new { codigosturma });
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
                return await conexao.QueryAsync<FrequenciaAlunoRetornoDto>(query, new { codigosturma, componenteCurricularId, bimestre });
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

            var parametros = new { codigosAluno, anoLetivo, modalidade, semestre };

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
                return await conexao.QueryAsync<string>(query, new { turmaCodigo, bimestre });
            }
        }

        public async Task<IEnumerable<FrequenciaAlunoConsolidadoDto>> ObterFrequenciaAlunosPorCodigoBimestre(string[] codigosAlunos, string bimestre, string turmaCodigo, TipoFrequenciaAluno tipoFrequencia, string componenteCurricularId)
        {
            var query = new StringBuilder(@"select 
	                                            extract(year from periodo_inicio) as AnoBimestre,
                                                fa.bimestre,
	                                            sum(fa.total_aulas) as TotalAula,
	                                            sum(fa.total_presencas) as TotalPresencas,
	                                            sum(fa.total_remotos) as TotalRemotos,
	                                            sum(fa.total_ausencias) as TotalAusencias,
	                                            sum(fa.total_compensacoes) as TotalCompensacoes,
	                                            fa.codigo_aluno as CodigoAluno
                                            from frequencia_aluno fa
                                            where not fa.excluido 
                                            and fa.tipo = @tipoFrequencia
                                            and fa.codigo_aluno = any(@codigosAlunos)  
                                            and fa.turma_id = @turmaCodigo ");
            if (!string.IsNullOrEmpty(componenteCurricularId))
                query.AppendLine(" and fa.disciplina_id = @componenteCurricularId");

            if (bimestre != "-99")
                query.AppendLine("and fa.bimestre = @numeroBimestre ");

            query.AppendLine(@" group by 
                                fa.bimestre,
                                fa.codigo_aluno,fa.periodo_inicio;");
            var parametros = new { codigosAlunos, numeroBimestre = Convert.ToInt32(bimestre), turmaCodigo, tipoFrequencia, componenteCurricularId };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<FrequenciaAlunoConsolidadoDto>(query.ToString(), parametros);
            }
        }

        public async Task<IEnumerable<AusenciaBimestreDto>> ObterAusenciaPorAlunoTurmaBimestre(string[] alunosCodigo, string turmaCodigo, string bimestre)
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

            query += @" and a.data_aula between pe.periodo_inicio and pe.periodo_fim 
                order by pe.bimestre,a.data_aula desc; ";

            var parametros = new
            {
                alunosCodigo,
                turmaCodigo,
                numeroBimestre = Convert.ToInt32(bimestre)
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

            var parametros = new { turmasCodigo, bimestre = int.Parse(bimestre), componenteCurricularId, tipoFrequencia = (int)TipoFrequenciaAluno.PorDisciplina };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<FrequenciaAlunoConsolidadoRelatorioDto>(query.ToString(), parametros);
            }
        }

        public async Task<IEnumerable<FrequenciaAlunoMensalConsolidadoDto>> ObterFrequenciaAlunoMensal(bool exibirHistorico, int anoLetivo, string codigoDre,
            string codigoUe, Modalidade modalidade, int semestre, string[] codigosTurmas, int[] mesesReferencias, int percentualAbaixoDe)
        {
            var query = @"select d.dre_id AS DreCodigo, d.abreviacao as DreSigla,
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
                query +=  " and d.dre_id = @codigoDre";

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
    }
}
