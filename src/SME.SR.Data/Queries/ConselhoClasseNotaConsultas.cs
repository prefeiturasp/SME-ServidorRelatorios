namespace SME.SR.Data
{
    public class ConselhoClasseNotaConsultas
    {
        internal static string NotasAlunoBimestre = @"select 
                         ccn.id, ccn.componente_curricular_codigo as ComponenteCurricularCodigo, 
                         ccn.conceito_id as ConceitoId, cv.valor as Conceito, ccn.nota
                          from conselho_classe_aluno cca 
                         inner join conselho_classe_nota ccn on ccn.conselho_classe_aluno_id = cca.id
                         left join conceito_valores cv on ccn.conceito_id = cv.id
                          where cca.aluno_codigo = @codigoAluno
                            and cca.conselho_classe_id = @conselhoClasseId	";

        internal static string NotasFinaisBimestre = @"select distinct * from (
                select pe.bimestre, fn.disciplina_id as ComponenteCurricularCodigo, 
                coalesce(ccn.conceito_id, fn.conceito_id) as ConceitoId, 
                coalesce(cvc.valor, cvf.valor) as Conceito, 
                coalesce(ccn.nota, fn.nota) as Nota
                  from fechamento_turma ft
                  left join periodo_escolar pe on pe.id = ft.periodo_escolar_id 
                 inner join turma t on t.id = ft.turma_id 
                 inner join fechamento_turma_disciplina ftd on ftd.fechamento_turma_id = ft.id
                 inner join fechamento_aluno fa on fa.fechamento_turma_disciplina_id = ftd.id
                 inner join fechamento_nota fn on fn.fechamento_aluno_id = fa.id
                 left join conceito_valores cvf on fn.conceito_id = cvf.id
                 inner join conselho_classe cc on cc.fechamento_turma_id = ft.id
                  left join conselho_classe_aluno cca on cca.conselho_classe_id  = cc.id
		                                        and cca.aluno_codigo = fa.aluno_codigo 
                  left join conselho_classe_nota ccn on ccn.conselho_classe_aluno_id = cca.id 
		                                        and ccn.componente_curricular_codigo = fn.disciplina_id 
                  left join conceito_valores cvc on ccn.conceito_id = cvc.id
                 where t.turma_id = @codigoTurma
                   and fa.aluno_codigo = @codigoAluno
                union all 
                select pe.bimestre, ccn.componente_curricular_codigo as ComponenteCurricularCodigo, 
                       coalesce(ccn.conceito_id, fn.conceito_id) as ConceitoId, 
                       coalesce(cvc.valor, cvf.valor) as Conceito, 
                       coalesce(ccn.nota, fn.nota) as Nota
                  from fechamento_turma ft
                  left join periodo_escolar pe on pe.id = ft.periodo_escolar_id 
                 inner join turma t on t.id = ft.turma_id 
                 inner join conselho_classe cc on cc.fechamento_turma_id = ft.id
                 inner join conselho_classe_aluno cca on cca.conselho_classe_id  = cc.id
                 inner join conselho_classe_nota ccn on ccn.conselho_classe_aluno_id = cca.id 
                  left join conceito_valores cvc on ccn.conceito_id = cvc.id
                  left join fechamento_turma_disciplina ftd on ftd.fechamento_turma_id = ft.id
                  left join fechamento_aluno fa on fa.fechamento_turma_disciplina_id = ftd.id
		                                        and cca.aluno_codigo = fa.aluno_codigo 
                  left join fechamento_nota fn on fn.fechamento_aluno_id = fa.id
		                                        and ccn.componente_curricular_codigo = fn.disciplina_id 
                  left join conceito_valores cvf on fn.conceito_id = cvf.id
                 where t.turma_id = @codigoTurma
                   and cca.aluno_codigo = @codigoAluno
                ) x	";
	}
}
