namespace SME.SR.Data
{
    public static class FrequenciaAlunoConsultas
    {
        private static readonly string CamposFrequencia = @"id Id, codigo_aluno CodigoAluno, 
                            tipo, disciplina_id DisciplinaId, periodo_inicio PeriodoInicio, 
                            periodo_fim PeriodoFim, bimestre, total_aulas TotalAulas, 
                            total_ausencias TotalAusencias, total_compensacoes TotalCompensacoes, 
                            turma_id TurmaId, periodo_escolar_id PeriodoEscolarId";

        internal static string FrequenciaGlobal = @"select 
        case when sum(total_aulas) > 0 then
	        coalesce((ROUND((100 - (cast((sum(total_ausencias) -  sum(total_compensacoes)) as decimal) / sum(total_aulas)) * 100), 2)),100) 
        else
            null 
        end FrequenciaGlobal
        from frequencia_aluno fa
       where tipo = 2
        and codigo_aluno = @codigoAluno
        and fa.turma_id = @codigoTurma";

        internal static string FrequenciGlobalPorBimestre = @"select pe.bimestre,
                case when sum(total_aulas) > 0 then
  	                coalesce((ROUND((100 - (cast((sum(total_ausencias) -  sum(total_compensacoes)) as decimal) / sum(total_aulas)) * 100), 2)),100) FrequenciaGlobal
                else
                    null 
                end FrequenciaGlobal
          	from tipo_calendario tc 
          		inner join periodo_escolar pe
          			on tc.id = pe.tipo_calendario_id 
          		left join frequencia_aluno fa
          			on pe.id = fa.periodo_escolar_id and
          			   fa.tipo = 2 and
          	    	   fa.codigo_aluno = @codigoAluno and
          	    	   fa.turma_id = @codigoTurma  				
          where tc.ano_letivo = @anoLetivo and
          	    tc.modalidade = @modalidade
          group by pe.bimestre;";

        internal static string FrequenciaPorAlunoDataDisciplina = @$"select {CamposFrequencia}
                        from frequencia_aluno
                        where
	                        codigo_aluno = @codigoAluno
	                        and tipo = @tipoFrequencia
	                        and periodo_inicio <= @dataAtual
	                        and periodo_fim >= @dataAtual
                            and disciplina_id = @disciplinaId
                            and turma_id = @codigoTurma";

        internal static string FrequenciaPorAlunoBimestreDisciplina = @$"select {CamposFrequencia}
                        from frequencia_aluno
                        where codigo_aluno = @codigoAluno
	                        and tipo = @tipoFrequencia
	                        and bimestre = @bimestre
                            and disciplina_id = @disciplinaId
                            and turma_id = @codigoTurma";

        public static string FrequenciaPorAlunoTurmaBimestre(int? bimestre)
        {
            var query = @$"select {CamposFrequencia} from frequencia_aluno fa 
                            where fa.codigo_aluno = @codigoAluno
                            and fa.turma_id = @codigoTurma and fa.tipo = 1";

            if (bimestre.HasValue)
                query += " and fa.bimestre = @bimestre";

            return query;
        }
    }
}
