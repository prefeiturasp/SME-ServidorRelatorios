namespace SME.SR.Data
{
    public static class FrequenciaAlunoConsultas
    {
        internal static string FrequenciaGlobal = @"select 
	    coalesce((ROUND((100 - (cast((sum(total_ausencias) -  sum(total_compensacoes)) as decimal) / sum(total_aulas)) * 100), 2)),0) FrequenciaGlobal
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
