namespace SME.SR.Data
{
    public static class ConselhoClasseAlunoConsultas
    {
        internal static string ParecerConclusivoPorTurma = @"select pe.bimestre
	            , cca.aluno_codigo as AlunoCodigo
	            , ccp.nome as ParecerConclusivo
              from fechamento_turma ft
             inner join turma t on t.id = ft.turma_id
              left join periodo_escolar pe on pe.id = ft.periodo_escolar_id
             inner join conselho_classe cc on cc.fechamento_turma_id = ft.id
             inner join conselho_classe_aluno cca on cca.conselho_classe_id = cc.id and not cca.excluido
             inner join conselho_classe_parecer ccp on ccp.id = cca.conselho_classe_parecer_id
            where t.turma_id = @turmaCodigo";

		internal static string ParecerConclusivo = @"SELECT
				ccp.nome
			FROM
				conselho_classe_aluno cca
			INNER JOIN conselho_classe_parecer ccp on cca.conselho_classe_parecer_id = ccp.id 
			WHERE
				cca.aluno_codigo = @codigoAluno and
				cca.conselho_classe_id = @conselhoClasseId
			ORDER BY cca.id";

		internal static string Recomendacoes = @"select cc.id as conselhoClasseId, cca.id as conselhoClasseAlunoId, cca.recomendacoes_aluno RecomendacoesAluno, 
			cca.recomendacoes_familia RecomendacoesFamilia, 
			cca.anotacoes_pedagogicas AnotacoesPedagogicas,
			ccr.id, ccr.recomendacao, ccr.tipo
			from conselho_classe cc
			inner join conselho_classe_aluno cca on cca.conselho_classe_id = cc.id 
			left join conselho_classe_aluno_recomendacao ccar on ccar.conselho_classe_aluno_id = cca.id
			left join conselho_classe_recomendacao ccr on ccr.id = ccar.conselho_classe_recomendacao_id
			where cc.fechamento_turma_id = @fechamentoTurmaId
				  and cca.aluno_codigo = @codigoAluno
			order by ccr.tipo, ccr.id	";

		internal static string ObterPorConselhoClasseId = @"SELECT
				1
			FROM
				conselho_classe_aluno cca
			WHERE
				cca.aluno_codigo = @codigoAluno and
				cca.conselho_classe_id = @conselhoClasseId	";
	}
}
