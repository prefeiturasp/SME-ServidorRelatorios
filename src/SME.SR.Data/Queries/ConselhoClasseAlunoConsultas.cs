namespace SME.SR.Data
{
    public static class ConselhoClasseAlunoConsultas
    {
		internal static string ParecerConclusivo = @"SELECT
				ccp.nome
			FROM
				conselho_classe_aluno cca
			INNER JOIN conselho_classe_parecer ccp on cca.conselho_classe_parecer_id = ccp.id 
			WHERE
				cca.aluno_codigo = @codigoAluno and
				cca.conselho_classe_id = @conselhoClasseId	";

		internal static string Recomendacoes = @"select cca.recomendacoes_aluno RecomendacoesAluno, 
			cca.recomendacoes_familia RecomendacoesFamilia, 
			cca.anotacoes_pedagogicas AnotacoesPedagogicas
			from conselho_classe cc
			inner join conselho_classe_aluno cca on cca.conselho_classe_id = cc.id 
			where cc.fechamento_turma_id = @fechamentoTurmaId
				  and cca.aluno_codigo = @codigoAluno";

		internal static string ObterPorConselhoClasseId = @"SELECT
				1
			FROM
				conselho_classe_aluno cca
			WHERE
				cca.aluno_codigo = @codigoAluno and
				cca.conselho_classe_id = @conselhoClasseId	";
	}
}
