namespace SME.SR.Workers.SGP.Infra
{
    public class FechamentoAlunoConsultas
    {
        internal static string AnotacoesAluno =
            @"select fa.anotacao, ftd.disciplina_id DisciplinaId, coc.descricao_eol Disciplina,
                case
                    when fa.alterado_por is null then fa.criado_por
                    when fa.alterado_por is not null then fa.alterado_por
                    end as professor,
                    case
                    when fa.alterado_em is null then fa.criado_em
                    when fa.alterado_em is not null then fa.alterado_em
                    end as data,
                    case 
                    when fa.alterado_rf is null then fa.criado_rf
                    when fa.alterado_rf is not null then fa.alterado_rf
                    end as professorrf
                from fechamento_turma_disciplina ftd 
                inner join fechamento_aluno fa on fa.fechamento_turma_disciplina_id = ftd.id
                inner join fechamento_turma ft on ftd.fechamento_turma_id = ft.id 
                inner join componente_curricular coc on ftd.disciplina_id = coc.codigo_eol 
                inner join turma t on ft.turma_id = t.id 
                where  fa.aluno_codigo = @codigoAluno and
                       ftd.fechamento_turma_id = @fechamentoTurmaId";
    }
}
