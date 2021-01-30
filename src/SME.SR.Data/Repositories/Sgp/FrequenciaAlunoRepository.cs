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

        private readonly string CamposFrequencia = @"id Id, codigo_aluno CodigoAluno, 
                            tipo, disciplina_id DisciplinaId, periodo_inicio PeriodoInicio, 
                            periodo_fim PeriodoFim, bimestre, total_aulas TotalAulas, 
                            total_ausencias TotalAusencias, total_compensacoes TotalCompensacoes, 
                            turma_id TurmaId, periodo_escolar_id PeriodoEscolarId";

        public FrequenciaAlunoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<FrequenciaAluno> ObterPorAlunoDataDisciplina(string codigoAluno, DateTime dataAtual, TipoFrequenciaAluno tipoFrequencia, string disciplinaId)
        {
            var query = FrequenciaAlunoConsultas.FrequenciaPorAlunoDataDisciplina;
            var parametros = new
            {
                CodigoAluno = codigoAluno,
                DataAtual = dataAtual,
                TipoFrequencia = tipoFrequencia,
                DisciplinaId = disciplinaId
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<FrequenciaAluno>(query, parametros);
            }
        }

        public async Task<double> ObterFrequenciaGlobal(string codigoTurma, string codigoAluno)
        {            
            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);

            var turma = await conexao.QueryFirstAsync<Turma>(TurmaConsultas.TurmaPorCodigo, new { codigoTurma });                        

            if (turma.AnoLetivo.Equals(2020))
            {                                          
                var percentuais = await conexao.QueryAsync<(int, double)>(FrequenciaAlunoConsultas.FrequenciGlobalPorBimestre, 
                    new { CodigoTurma = codigoTurma, CodigoAluno = codigoAluno, turma.AnoLetivo, modalidade = turma.ModalidadeTipoCalendario });

                return percentuais.Any() ? Math.Round(percentuais.Sum(p => p.Item2) / percentuais.Count(), 2) : 100;
            }

            var parametros = new { CodigoTurma = codigoTurma, CodigoAluno = codigoAluno };
            return await conexao.QueryFirstOrDefaultAsync<double>(FrequenciaAlunoConsultas.FrequenciaGlobal, parametros);

        }

        public async Task<FrequenciaAluno> ObterPorAlunoBimestreAsync(string codigoAluno, int bimestre, TipoFrequenciaAluno tipoFrequencia, string disciplinaId)
        {
            var query = FrequenciaAlunoConsultas.FrequenciaPorAlunoBimestreDisciplina;
            var parametros = new
            {
                CodigoAluno = codigoAluno,
                Bimestre = bimestre,
                TipoFrequencia = tipoFrequencia,
                DisciplinaId = disciplinaId
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

        public async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciasPorTurmasAlunos(string[] codigosTurma, string[] codigosAluno)
        {
            var query = @$"select {CamposFrequencia} from frequencia_aluno fa 
                            where fa.codigo_aluno = ANY(@codigosAluno)
                            and fa.turma_id = ANY(@codigosTurma) and fa.tipo = 1";

            var parametros = new { CodigosTurma = codigosTurma, CodigosAluno = codigosAluno };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<FrequenciaAluno>(query, parametros);
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
                return await conexao.QueryAsync<FrequenciaAluno>(query.ToString(), new { componentesCurriculares, bimestres, turmasCodigos});
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
    }
}
