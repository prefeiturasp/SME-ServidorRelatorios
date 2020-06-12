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

        internal static string FechamentosTurmaPorCodigoTurma = @" f.id, f.turma_id TurmaId, f.periodo_escolar_id PeriodoEscolarId,
                      t.turma_id CodigoTurma, t.nome, t.ano_letivo AnoLetivo, 
                      t.modalidade_codigo ModalidadeCodigo, t.Semestre,
                      p.bimestre, p.periodo_inicio PeriodoInicio, p.periodo_fim PeriodoFim
                       from fechamento_turma f
                       inner join turma t on t.id = f.turma_id
                       left join periodo_escolar p on p.id = f.periodo_escolar_id
                       left join tipo_calendario tp on tp.id = p.tipo_calendario_id 
                       where not f.excluido  
                       and t.turma_id = @turmaCodigo"
;
    }
}
