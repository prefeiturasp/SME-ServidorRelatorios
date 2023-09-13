using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class RegistroFrequenciaAlunoRepository : IRegistroFrequenciaAlunoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public RegistroFrequenciaAlunoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<RegistroFrequenciaAlunoDto>> ObterRegistrosFrequenciasAluno(string[] codigosAlunos, string[] turmasCodigo, string[] componentesCurricularesId, long tipoCalendarioId, int[] bimestres)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(@"select
	                    rfa.codigo_aluno as CodigoAluno,
	                    a.disciplina_id as ComponenteCurricularId,
	                    rfa.valor as TipoFrequencia,
	                    t.turma_id as TurmaCodigo,
	                    t.ano_letivo as AnoTurma,
	                    t.modalidade_codigo as ModalidadeTurma,
	                    pe.periodo_inicio as PeriodoInicio,
	                    pe.periodo_fim as PeriodoFim,
	                    pe.bimestre as Bimestre,
	                    sum(rfa.numero_aula) as TotalAulas
                    from
	                    registro_frequencia_aluno rfa
                    inner join registro_frequencia rf on
	                    rfa.registro_frequencia_id = rf.id
                    inner join aula a on
	                    rf.aula_id = a.id
                    inner join turma t on
	                    a.turma_id = t.turma_id
                    inner join periodo_escolar pe on
	                    a.tipo_calendario_id = pe.tipo_calendario_id
                    where
	                    codigo_aluno = ANY(@codigosAlunos) and (a.data_aula between pe.periodo_inicio and pe.periodo_fim)
                    ");

            if (tipoCalendarioId > 0)
                query.AppendLine(" and a.tipo_calendario_id = @tipoCalendarioId ");
            if (bimestres.Length > 0)
                query.AppendLine(" and p.bimestre = any(@bimestres) ");
            if (componentesCurricularesId.Length > 0)
                query.AppendLine("and a.disciplina_id = any(@componentesCurricularesId) ");

            query.AppendLine(@"and a.turma_id = any(@turmasCodigo) group by
                        rfa.codigo_aluno,
                        a.disciplina_id,
                        rfa.valor,
                        t.turma_id,
                        t.ano_letivo,
                        t.modalidade_codigo,
                        pe.periodo_inicio,
                        pe.periodo_fim,
                        pe.bimestre");
            
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<RegistroFrequenciaAlunoDto>(query.ToString(), new { codigosAlunos, turmasCodigo, componentesCurricularesId, tipoCalendarioId, bimestres });
            }
        }

        public async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaGeralAlunoPorAnoModalidadeSemestre(string alunoCodigo, int anoTurma, long tipoCalendarioId)
        {
            var query = new StringBuilder($@"with lista as (select fa.*, row_number() over (partition by fa.codigo_aluno, fa.turma_id, fa.bimestre order by fa.id desc) sequencia
                            from frequencia_aluno fa
                            inner join turma t on fa.turma_id = t.turma_id ");

            if (tipoCalendarioId > 0)
                query.AppendLine("inner join periodo_escolar pe on fa.periodo_escolar_id = pe.id");

            query.AppendLine(@" where fa.tipo = 2 
                and fa.codigo_aluno = @alunoCodigo 
                and t.ano_letivo = @anoTurma 
                and t.tipo_turma not in(3) ");

            if (tipoCalendarioId > 0)
                query.AppendLine(" and pe.tipo_calendario_id = @tipoCalendarioId");

            query.AppendLine(@") select bimestre, codigo_aluno as CodigoAluno, total_aulas as TotalAulas, total_ausencias as TotalAusencias,
                                 total_compensacoes as TotalCompensacoes, total_remotos as TotalRemotos from lista where sequencia = 1;");

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao
                .QueryAsync<FrequenciaAluno>(query.ToString(), new
                {
                    alunoCodigo,
                    anoTurma,
                    tipoCalendarioId
                });
            }
        }
    }
}
