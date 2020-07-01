namespace SME.SR.Data
{
    public static class ComponenteCurricularConsultas
    {
        internal static string BuscarPorTurmas = @"select distinct iif(pcc.cd_componente_curricular is not null, pcc.cd_componente_curricular,
                                        cc.cd_componente_curricular) as Codigo,
                                    iif(pcc.dc_componente_curricular is not null, pcc.dc_componente_curricular,
                                        cc.dc_componente_curricular) as Descricao,
										serie_ensino.sg_resumida_serie   as AnoTurma,
                                        dtt.qt_hora_duracao              as TurnoTurma,
                                        te.cd_turma_escola               as CodigoTurma
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
                    where te.cd_turma_escola in @codigosTurma
                      and te.st_turma_escola in ('O', 'A', 'C')";

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

        internal static string BuscarPorTurmasEProfessor = @"select distinct iif(pcc.cd_componente_curricular is not null, pcc.cd_componente_curricular,
                                                                cc.cd_componente_curricular) as Codigo,
                                                            iif(pcc.dc_componente_curricular is not null, pcc.dc_componente_curricular,
                                                                cc.dc_componente_curricular) as Descricao,
                                                            esc.tp_escola                    as TipoEscola,
                                                            dtt.qt_hora_duracao              as TurnoTurma,
				                                            serie_ensino.sg_resumida_serie   as AnoTurma,
                                                            te.cd_turma_escola               as CodigoTurma
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
									where te.cd_turma_escola in @codigosTurma
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

        internal static string BuscarTerritorioDoSaberTurmas = @"select
						grade_ter.cd_experiencia_pedagogica as CodigoExperienciaPedagogica,
						grade_ter.cd_territorio_saber as CodigoTerritorioSaber,
						ter.dc_territorio_saber as DescricaoTerritorioSaber,
						exp.dc_experiencia_pedagogica as DescricaoExperienciaPedagogica,
						Convert(date, dt_inicio) as DataInicio,
						grade_ter.cd_componente_curricular as CodigoComponenteCurricular,
                        grade_tur.cd_turma_escola as CodigoTurma
					from
						turma_grade_territorio_experiencia grade_ter
						inner join território_saber ter on ter.cd_territorio_saber = grade_ter.cd_territorio_saber
						inner join tipo_experiencia_pedagogica exp on exp.cd_experiencia_pedagogica = grade_ter.cd_experiencia_pedagogica
                        inner join serie_turma_grade grade_tur on grade_tur.cd_serie_grade = grade_ter.cd_serie_grade
					where grade_tur.cd_turma_escola in @codigoTurma and
						grade_ter.cd_componente_curricular in @codigosComponentesCurriculares";

        internal static string Listar = @"SELECT 
                        cd_componente_curricular AS Codigo, 
                        RTRIM(LTRIM(dc_componente_curricular)) AS Descricao,
                        0 as TerritorioSaber,
                        CASE
                            WHEN cd_componente_curricular IN (508, 511, 1064, 1065, 1104, 1105, 1112, 1113, 1114, 1115, 1117, 1121, 1124, 1125, 1211, 1212, 1213, 1290, 1301) THEN 1
                            ELSE 0
                        END EhRegencia
                    FROM componente_curricular";

        internal static string BuscarTerritorioAgrupado(string[] ids) { 
        
            return $@"select distinct(convert(bigint,concat(stg.cd_turma_escola, grade_ter.cd_territorio_saber, grade_ter.cd_experiencia_pedagogica, FORMAT(grade_ter.dt_inicio, 'MM'), FORMAT(grade_ter.dt_inicio, 'dd')))) as CdComponenteCurricular,
                    concat( ter.dc_territorio_saber, ' - ',exp.dc_experiencia_pedagogica)  as Descricao, 
                    0 as EhRegencia,
                    1 as Territorio
                    from  turma_grade_territorio_experiencia grade_ter inner join território_saber ter on ter.cd_territorio_saber = grade_ter.cd_territorio_saber 
                    inner join tipo_experiencia_pedagogica exp on exp.cd_experiencia_pedagogica = grade_ter.cd_experiencia_pedagogica
                    inner join serie_turma_grade stg on stg.cd_serie_grade = grade_ter.cd_serie_grade
                    where convert(bigint,concat(stg.cd_turma_escola, grade_ter.cd_territorio_saber, grade_ter.cd_experiencia_pedagogica, FORMAT(grade_ter.dt_inicio, 'MM'), FORMAT(grade_ter.dt_inicio, 'dd'))) IN ({string.Join(',', ids)})";
        } 

        internal static string ListarApiEol = @"SELECT IdComponenteCurricular, 
                            IdComponenteCurricularPai, 
                            EhCompartilhada, 
                            EhRegencia, 
                            PermiteRegistroFrequencia, 
                            PermiteLancamentoDeNota,
                            EhTerritorio,
                            EhBaseNacional,
                            IdGrupoMatriz
                    FROM ComponenteCurricular";

        internal static string ListarRegencia = @"
				SELECT 
					IdComponenteCurricular
					,Turno
					,Ano
					,Idgrupomatriz 
				FROM RegenciaComponenteCurricular";

        internal static string ListarGruposMatriz = @"select id, nome from componentecurriculargrupomatriz";
    }
}
