namespace SME.SR.Data
{
    public static class FrequenciaAlunoConsultas
    {
        internal static string FrequenciaGlobal = @"select 
	    coalesce((ROUND((100 - (cast((sum(total_ausencias) -  sum(total_compensacoes)) as decimal) / sum(total_aulas)) * 100), 2)),100) FrequenciaGlobal
        from frequencia_aluno fa
       where tipo = 2
        and codigo_aluno = @codigoAluno
        and fa.turma_id = @codigoTurma";

        internal static string FrequenciaPorAlunoDataDisciplina = @"select *
                        from frequencia_aluno
                        where
	                        codigo_aluno = @codigoAluno
	                        and tipo = @tipoFrequencia
	                        and periodo_inicio <= @dataAtual
	                        and periodo_fim >= @dataAtual
                            and disciplina_id = @disciplinaId";

        internal static string FrequenciaPorAlunoBimestreDisciplina = @"select *
                        from frequencia_aluno
                        where codigo_aluno = @codigoAluno
	                        and tipo = @tipoFrequencia
	                        and bimestre = @bimestre
                            and disciplina_id = @disciplinaId";

        internal static string FrequenciaDisciplinaGlobalPorTurma = @"select fa.codigo_aluno as CodigoAluno
                                , fa.disciplina_id as DisciplinaId
                                , sum(fa.total_aulas) as TotalAulas
                                , sum(fa.total_ausencias) as TotalAusencias
                                , sum(fa.total_compensacoes) as TotalCompensacoes
                            from frequencia_aluno fa
                           inner join periodo_escolar pe on pe.id = fa.periodo_escolar_id
                            where fa.tipo = 1
                              and fa.turma_id = @turmaCodigo
                              and pe.tipo_calendario_id = @tipoCalendarioId
                            group by fa.codigo_aluno, fa.disciplina_id ";

        public static string FrequenciaPorAlunoTurmaBimestre(int? bimestre)
        {
            var query = @"select * 
                            from frequencia_aluno fa 
                            where fa.codigo_aluno = @codigoAluno
                            and fa.turma_id = @codigoTurma and fa.tipo = 1";

            if (bimestre.HasValue)
                query += " and fa.bimestre = @bimestre";

            return query;
        }
    }
}
