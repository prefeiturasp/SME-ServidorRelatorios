namespace SME.SR.Data
{
    public class AulaConsultas
    {
		internal static string AulasCumpridas = @"select 
                         coalesce(SUM(a.quantidade) filter (where a.tipo_aula = 1 and rf.id is not null), 0) as Cumpridas
                         from periodo_escolar p
                         inner join tipo_calendario tp on p.tipo_calendario_id = tp.id
                         left join aula a on a.tipo_calendario_id = p.tipo_calendario_id and
				                        a.data_aula BETWEEN p.periodo_inicio AND p.periodo_fim
                                        and (a.id is null or not a.excluido)
                         left join registro_frequencia rf on a.id = rf.aula_id
                          where tp.situacao and not tp.excluido and
                                a.tipo_calendario_id = @tipoCalendarioId and
                                a.turma_id = @codigoTurma and
                                a.disciplina_id = @disciplinaId and
                                p.bimestre = @bimestre
                            ";
	}
}
