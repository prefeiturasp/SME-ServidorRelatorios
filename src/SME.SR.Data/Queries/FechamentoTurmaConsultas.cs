namespace SME.SR.Data
{
    public class FechamentoTurmaConsultas
    {
		internal static string FechamentoTurmaPeriodo = @"select 
                          f.turma_id TurmaId, f.periodo_escolar_id PeriodoEscolarId,
                          t.turma_id CodigoTurma, t.nome, t.ano_letivo AnoLetivo, 
                          t.modalidade_codigo ModalidadeCodigo, t.Semestre,
                          pe.bimestre, pe.periodo_inicio PeriodoInicio, pe.periodo_fim PeriodoFim
                           from fechamento_turma f
                          inner join turma t on t.id = f.turma_id
                           left join periodo_escolar pe on pe.id = f.periodo_escolar_id
                          where f.id = @fechamentoTurmaId";
	}
}
