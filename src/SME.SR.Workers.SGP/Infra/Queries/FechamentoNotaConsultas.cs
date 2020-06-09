namespace SME.SR.Workers.SGP.Infra
{
    public class FechamentoNotaConsultas
    {
        internal static string NotasAlunoBimestre = @"select 
                         fn.disciplina_id as ComponenteCurricularCodigo, fn.conceito_id as ConceitoId, 
                         fn.nota, pe.bimestre 
                          from fechamento_turma ft
                         inner join turma t on t.id = ft.turma_id 
                          left join periodo_escolar pe on pe.id = ft.periodo_escolar_id 
                         inner join fechamento_turma_disciplina ftd on ftd.fechamento_turma_id = ft.id
                         inner join fechamento_aluno fa on fa.fechamento_turma_disciplina_id = ftd.id
                         inner join fechamento_nota fn on fn.fechamento_aluno_id = fa.id
                          where ftd.fechamento_turma_id = @fechamentoTurmaId
                           and fa.aluno_codigo = @codigoAluno   ";
    }
}
