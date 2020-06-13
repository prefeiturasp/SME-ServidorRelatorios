namespace SME.SR.Data
{
    public static class ComponenteCurricularConsultas
    {
		internal static string BuscarPorTurma = @"select distinct iif(pcc.cd_componente_curricular is not null, pcc.cd_componente_curricular,
                                        cc.cd_componente_curricular) as Codigo,
                                    iif(pcc.dc_componente_curricular is not null, pcc.dc_componente_curricular,
                                        cc.dc_componente_curricular) as Descricao,
										serie_ensino.sg_resumida_serie   as AnoTurma,
                                        dtt.qt_hora_duracao              as TurnoTurma
                    from turma_escola te
                             inner join escola esc ON te.cd_escola = esc.cd_escola
                             inner join v_cadastro_unidade_educacao ue on ue.cd_unidade_educacao = esc.cd_escola
                             inner join unidade_administrativa dre on dre.tp_unidade_administrativa = 24
                        and ue.cd_unidade_administrativa_referencia = dre.cd_unidade_administrativa

                        --Serie Ensino
                             left join serie_turma_escola ON serie_turma_escola.cd_turma_escola = te.cd_turma_escola
                             left join serie_turma_grade ON serie_turma_grade.cd_turma_escola = serie_turma_escola.cd_turma_escola
                             left join escola_grade ON serie_turma_grade.cd_escola_grade = escola_grade.cd_escola_grade
                             left join grade ON escola_grade.cd_grade = grade.cd_grade
                             left join grade_componente_curricular gcc on gcc.cd_grade = grade.cd_grade
                             left join componente_curricular cc on cc.cd_componente_curricular = gcc.cd_componente_curricular
                        and cc.dt_cancelamento is null
                             left join serie_ensino
                                       ON grade.cd_serie_ensino = serie_ensino.cd_serie_ensino

                        -- Programa
                             left join tipo_programa tp on te.cd_tipo_programa = tp.cd_tipo_programa
                             left join turma_escola_grade_programa tegp on tegp.cd_turma_escola = te.cd_turma_escola
                             left join escola_grade teg on teg.cd_escola_grade = tegp.cd_escola_grade
                             left join grade pg on pg.cd_grade = teg.cd_grade
                             left join grade_componente_curricular pgcc on pgcc.cd_grade = teg.cd_grade
                             left join componente_curricular pcc on pgcc.cd_componente_curricular = pcc.cd_componente_curricular
                        and pcc.dt_cancelamento is null
                        -- Turno     
                             inner join duracao_tipo_turno dtt on te.cd_tipo_turno = dtt.cd_tipo_turno and te.cd_duracao = dtt.cd_duracao
                    where te.cd_turma_escola = @codigoTurma
                      and te.st_turma_escola in ('O', 'A', 'C')";

        internal static string Listar = @"SELECT IdComponenteCurricular, 
                            IdComponenteCurricularPai, 
                            EhCompartilhada, 
                            EhRegencia, 
                            PermiteRegistroFrequencia, 
                            PermiteLancamentoDeNota,
                            EhTerritorio,
                            EhBaseNacional,
                            IdGrupoMatriz
                    FROM ComponenteCurricular";

        internal static string ListarGruposMatriz = @"select id, nome from componentecurriculargrupomatriz";
    }
}
