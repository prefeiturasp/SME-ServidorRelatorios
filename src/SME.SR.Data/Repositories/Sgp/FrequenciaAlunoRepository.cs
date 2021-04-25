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
    public class FrequenciaAlunoRepository : IFrequenciaAlunoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        private readonly string CamposFrequencia = @"fa.id Id, fa.codigo_aluno CodigoAluno, 
                            fa.tipo, fa.disciplina_id DisciplinaId, fa.periodo_inicio PeriodoInicio, 
                            fa.periodo_fim PeriodoFim, fa.bimestre, fa.total_aulas TotalAulas, 
                            fa.total_ausencias TotalAusencias, fa.total_compensacoes TotalCompensacoes, 
                            fa.turma_id TurmaId, fa.periodo_escolar_id PeriodoEscolarId";

        public FrequenciaAlunoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<FrequenciaAluno> ObterPorAlunoDataDisciplina(string codigoAluno, DateTime dataAtual, TipoFrequenciaAluno tipoFrequencia, string disciplinaId, string codigoTurma)
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

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<FrequenciaAluno>(query, parametros);
            }
        }

        public async Task<double> ObterFrequenciaGlobal(string codigoTurma, string codigoAluno)
        {
            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);

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

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<FrequenciaAluno>(query, parametros);
            }
        }

        public async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaPorDisciplinaBimestres(string codigoTurma, string codigoAluno, int? bimestre)
        {
            var query = FrequenciaAlunoConsultas.FrequenciaPorAlunoTurmaBimestre(bimestre);
            var parametros = new { CodigoTurma = codigoTurma, CodigoAluno = codigoAluno, Bimestre = bimestre };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<FrequenciaAluno>(query, parametros);
            }
        }

        public async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaDisciplinaGlobalPorTurma(string turmaCodigo, long tipoCalendarioId)
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
                              and fa.turma_id = @turmaCodigo
                              and pe.tipo_calendario_id = @tipoCalendarioId ";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<FrequenciaAluno>(query, new { turmaCodigo, tipoCalendarioId });
            }
        }

        public async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciasPorTurmasAlunos(string[] codigosAluno, int anoLetivo, int modalidade, int semestre)
        {
            var query = @$"select fa.codigo_aluno CodigoAluno, t.ano_letivo as AnoTurma, t.modalidade_codigo as ModalidadeTurma,
                            fa.tipo, fa.disciplina_id DisciplinaId, fa.periodo_inicio PeriodoInicio, 
                            fa.periodo_fim PeriodoFim, fa.bimestre, sum(fa.total_aulas) TotalAulas, 
                            sum(fa.total_ausencias) TotalAusencias, sum(fa.total_compensacoes) TotalCompensacoes, 
                            fa.periodo_escolar_id PeriodoEscolarId
                             from frequencia_aluno fa 
                            inner join turma t on t.turma_id = fa.turma_id
                            where fa.codigo_aluno = ANY(@codigosAluno)
                              and fa.tipo = 1
                              and t.ano_letivo = @anoLetivo
                              and t.modalidade_codigo = @modalidade
                              and t.semestre = @semestre
                            group by fa.codigo_aluno, fa.tipo, fa.disciplina_id, fa.periodo_inicio, 
                            fa.periodo_fim, fa.bimestre, fa.periodo_escolar_id, t.ano_letivo, t.modalidade_codigo";

            var parametros = new { codigosAluno, anoLetivo, modalidade, semestre };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<FrequenciaAluno>(query, parametros);
            }
        }

        public async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciasPorTurmasAlunosParaHistoricoEscolar(string[] codigosAluno, int anoLetivo, int modalidade, int semestre)
        {
            var query = new StringBuilder(@$"select fa.codigo_aluno CodigoAluno, t.ano_letivo as AnoTurma, t.modalidade_codigo as ModalidadeTurma,
                            fa.tipo, fa.disciplina_id DisciplinaId, fa.periodo_inicio PeriodoInicio, 
                            fa.periodo_fim PeriodoFim, fa.bimestre, sum(fa.total_aulas) TotalAulas, 
                            sum(fa.total_ausencias) TotalAusencias, sum(fa.total_compensacoes) TotalCompensacoes, 
                            fa.periodo_escolar_id PeriodoEscolarId
                             from frequencia_aluno fa 
                            inner join turma t on t.turma_id = fa.turma_id
                            where fa.codigo_aluno = ANY(@codigosAluno)
                              and fa.tipo = 1
                              and t.ano_letivo = @anoLetivo ");

            if (modalidade > 0)
            {
                query.AppendLine(" and t.modalidade_codigo = @modalidade ");
            }

            if (semestre > 0)
            {
                query.AppendLine(" and t.semestre = @semestre ");
            }

            query.AppendLine(@" group by fa.codigo_aluno, fa.tipo, fa.disciplina_id, fa.periodo_inicio, 
                                fa.periodo_fim, fa.bimestre, fa.periodo_escolar_id, t.ano_letivo, t.modalidade_codigo");

            var parametros = new { codigosAluno, anoLetivo, modalidade, semestre };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
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
                            group by fa.codigo_aluno";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
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

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<FrequenciaAluno>(query.ToString(), new { componentesCurriculares, bimestres, turmasCodigos });
            }
        }

        public async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaGeralAlunosPorTurma(string codigoTurma)
        {
            var query = @$"select fa.id Id
                                , fa.codigo_aluno as CodigoAluno
                                , fa.turma_id as TurmaId
                                , fa.total_aulas as TotalAulas
                                , fa.total_ausencias as TotalAusencias
                                , fa.total_compensacoes as TotalCompensacoes
                              from frequencia_aluno fa 
                            where fa.turma_id = @codigoTurma and fa.tipo = 2 ";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<FrequenciaAluno>(query, new { codigoTurma });
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

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
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

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<DateTime?>(query, new { professorRf });
            }
        }
    }
}
