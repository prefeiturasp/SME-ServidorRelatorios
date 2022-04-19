namespace SME.SR.Data
{
    public static class ComponenteCurricularConsultas
    {
        internal static string BuscarPorAlunos = @"

                    IF OBJECT_ID('tempdb..#tmp_matricula_cotic') IS NOT NULL
                    DROP TABLE #tmp_matricula_cotic

                    select cd_matricula, cd_aluno 
                    into #tmp_matricula_cotic
                    from v_matricula_cotic
                    where cd_aluno in @alunosCodigos

                    select distinct iif(pcc.cd_componente_curricular is not null, pcc.cd_componente_curricular,
                                        cc.cd_componente_curricular) as Codigo,
                                    iif(pcc.dc_componente_curricular is not null, pcc.dc_componente_curricular,
                                        cc.dc_componente_curricular) as Descricao,
										serie_ensino.sg_resumida_serie   as AnoTurma,
                                        dtt.qt_hora_duracao              as TurnoTurma,
                                        te.cd_turma_escola               as CodigoTurma,
                                        vmc.cd_aluno                    as CodigoAluno
                    from turma_escola te

	                inner join matricula_turma_escola mte on mte.cd_turma_escola = te.cd_turma_escola 
	                inner join #tmp_matricula_cotic vmc on vmc.cd_matricula = mte.cd_matricula 

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
                    where te.an_letivo = @anoLetivo
                      and te.st_turma_escola in ('O', 'A', 'C') and serie_turma_grade.dt_fim is null and te.cd_tipo_turma <> 3 ";

        internal static string BuscarPorAlunosHistorico = @"
                        
                        IF OBJECT_ID('tempdb..#tmp_aluno_cotic') IS NOT NULL
                        DROP TABLE #tmp_aluno_cotic

                        select cd_aluno 
                        into #tmp_aluno_cotic
                        from v_aluno_cotic
                        where cd_aluno in @alunosCodigos

                        select distinct iif(pcc.cd_componente_curricular is not null, pcc.cd_componente_curricular,
                                        cc.cd_componente_curricular) as Codigo,
                                    iif(pcc.dc_componente_curricular is not null, pcc.dc_componente_curricular,
                                        cc.dc_componente_curricular) as Descricao,
										serie_ensino.sg_resumida_serie   as AnoTurma,
                                        dtt.qt_hora_duracao              as TurnoTurma,
                                        te.cd_turma_escola               as CodigoTurma,
                                        aluno.cd_aluno                    as CodigoAluno
                        FROM #tmp_aluno_cotic aluno
						INNER JOIN v_historico_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
						INNER JOIN historico_matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
                        INNER JOIN turma_escola te ON mte.cd_turma_escola = te.cd_turma_escola					

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
                    where te.an_letivo = @anoLetivo
                      and te.st_turma_escola in ('O', 'A', 'C') and te.cd_tipo_turma <> 3";

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

        internal static string BuscarPorTurmaEProfessor = @"select distinct iif(pcc.cd_componente_curricular is not null, pcc.cd_componente_curricular,
                                                                cc.cd_componente_curricular) as Codigo,
                                                            iif(pcc.dc_componente_curricular is not null, pcc.dc_componente_curricular,
                                                                cc.dc_componente_curricular) as Descricao,
                                                            esc.tp_escola                    as TipoEscola,
                                                            dtt.qt_hora_duracao              as TurnoTurma,
				                                            serie_ensino.sg_resumida_serie   as AnoTurma
                                            from turma_escola (nolock) te
                                                     inner join escola (nolock) esc ON te.cd_escola = esc.cd_escola
                                                --Serie Ensino
                                                     left join serie_turma_escola (nolock) ON serie_turma_escola.cd_turma_escola = te.cd_turma_escola
                                                     left join serie_turma_grade (nolock) ON serie_turma_grade.cd_turma_escola = serie_turma_escola.cd_turma_escola
                                                     left join escola_grade (nolock) ON serie_turma_grade.cd_escola_grade = escola_grade.cd_escola_grade
                                                     left join grade (nolock) ON escola_grade.cd_grade = grade.cd_grade
                                                     left join grade_componente_curricular (nolock) gcc on gcc.cd_grade = grade.cd_grade
                                                     left join componente_curricular (nolock) cc on cc.cd_componente_curricular = gcc.cd_componente_curricular
                                                and cc.dt_cancelamento is null
                                                     left join serie_ensino (nolock)
                                                               ON grade.cd_serie_ensino = serie_ensino.cd_serie_ensino
                                                -- Programa
                                                     left join tipo_programa (nolock) tp on te.cd_tipo_programa = tp.cd_tipo_programa
                                                     left join turma_escola_grade_programa (nolock) tegp on tegp.cd_turma_escola = te.cd_turma_escola
                                                     left join escola_grade (nolock) teg on teg.cd_escola_grade = tegp.cd_escola_grade
                                                     left join grade (nolock) pg on pg.cd_grade = teg.cd_grade
                                                     left join grade_componente_curricular (nolock) pgcc on pgcc.cd_grade = teg.cd_grade
                                                     left join componente_curricular (nolock) pcc on pgcc.cd_componente_curricular = pcc.cd_componente_curricular
                                                and pcc.dt_cancelamento is null
                                                --Atribuição
                                                     inner join atribuicao_aula (nolock) aa
                                                                on (gcc.cd_grade = aa.cd_grade and gcc.cd_componente_curricular = aa.cd_componente_curricular
                                                                                               and aa.cd_serie_grade = serie_turma_grade.cd_serie_grade)
                                                                    or
                                                                   (pgcc.cd_grade = aa.cd_grade and pgcc.cd_componente_curricular = aa.cd_componente_curricular)
                                                                 and aa.dt_cancelamento is null and aa.dt_disponibilizacao_aulas is null and
                                                                   aa.an_atribuicao = year(getdate())
                                                     inner join v_cargo_base_cotic (nolock) vcbc on aa.cd_cargo_base_servidor = vcbc.cd_cargo_base_servidor
                                                     inner join v_servidor_cotic (nolock) vsc on vcbc.cd_servidor = vsc.cd_servidor
                                            --          left join funcao_atividade_cargo_servidor facs on aa.cd_cargo_base_servidor = facs.cd_cargo_base_servidor
                                            --     and facs.dt_cancelamento is null and facs.dt_fim_funcao_atividade is null
                                                    inner join duracao_tipo_turno dtt on te.cd_tipo_turno = dtt.cd_tipo_turno and te.cd_duracao = dtt.cd_duracao
									where te.cd_turma_escola = @codigoTurma
									  and te.st_turma_escola in ('O', 'A', 'C')
									  and vsc.cd_registro_funcional = @login";

        internal static string BuscarTerritorioDoSaber = @"select
						grade_ter.cd_experiencia_pedagogica as CodigoExperienciaPedagogica,
						grade_ter.cd_territorio_saber as CodigoTerritorioSaber,
						ter.dc_territorio_saber as DescricaoTerritorioSaber,
						exp.dc_experiencia_pedagogica as DescricaoExperienciaPedagogica,
						Convert(date, dt_inicio) as DataInicio,
						grade_ter.cd_componente_curricular as CodigoComponenteCurricular
					from
						turma_grade_territorio_experiencia grade_ter
						inner join território_saber ter on ter.cd_territorio_saber = grade_ter.cd_territorio_saber
						inner join tipo_experiencia_pedagogica exp on exp.cd_experiencia_pedagogica = grade_ter.cd_experiencia_pedagogica
					where
						exists (
							select
								*
							from
								serie_turma_grade grade_tur
							where
								cd_turma_escola = @codigoTurma
								and grade_tur.cd_serie_grade = grade_ter.cd_serie_grade
						)
						and grade_ter.cd_componente_curricular in @codigosComponentesCurriculares";
    }
}
