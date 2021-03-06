﻿using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ComponenteCurricularRepository : IComponenteCurricularRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ComponenteCurricularRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<ComponenteCurricular>> ObterComponentesPorTurma(string codigoTurma)
        {
            var query = ComponenteCurricularConsultas.BuscarPorTurma;
            var parametros = new { CodigoTurma = codigoTurma };

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            return await conexao.QueryAsync<ComponenteCurricular>(query, parametros);
        }

        public async Task<IEnumerable<ComponenteCurricularApiEol>> ListarApiEol()
        {
            var query = @"SELECT IdComponenteCurricular, 
                            IdComponenteCurricularPai, 
                            EhCompartilhada, 
                            EhRegencia, 
                            PermiteRegistroFrequencia, 
                            PermiteLancamentoDeNota,
                            EhTerritorio,
                            EhBaseNacional,
                            IdGrupoMatriz,
                            Descricao
                    FROM ComponenteCurricular";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringApiEol);
            return await conexao.QueryAsync<ComponenteCurricularApiEol>(query);
        }

        public async Task<IEnumerable<ComponenteCurricularRegenciaApiEol>> ListarRegencia()
        {
            var query = @"SELECT 
                            IdComponenteCurricular
					        ,Turno
					        ,Ano
					        ,Idgrupomatriz
                        FROM RegenciaComponenteCurricular";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringApiEol);
            return await conexao.QueryAsync<ComponenteCurricularRegenciaApiEol>(query);
        }

        public async Task<IEnumerable<Data.ComponenteCurricular>> ListarComponentesTerritorioSaber(string[] ids)
        {
            var query = $@"select distinct(convert(bigint,concat(stg.cd_turma_escola, grade_ter.cd_territorio_saber, grade_ter.cd_experiencia_pedagogica, FORMAT(grade_ter.dt_inicio, 'MM'), FORMAT(grade_ter.dt_inicio, 'dd')))) as CdComponenteCurricular,
                    concat( ter.dc_territorio_saber, ' - ',exp.dc_experiencia_pedagogica)  as Descricao, 
                    0 as EhRegencia,
                    1 as Territorio
                    from  turma_grade_territorio_experiencia grade_ter inner join território_saber ter on ter.cd_territorio_saber = grade_ter.cd_territorio_saber 
                    inner join tipo_experiencia_pedagogica exp on exp.cd_experiencia_pedagogica = grade_ter.cd_experiencia_pedagogica
                    inner join serie_turma_grade stg on stg.cd_serie_grade = grade_ter.cd_serie_grade
                    where convert(bigint,concat(stg.cd_turma_escola, grade_ter.cd_territorio_saber, grade_ter.cd_experiencia_pedagogica, FORMAT(grade_ter.dt_inicio, 'MM'), FORMAT(grade_ter.dt_inicio, 'dd'))) IN ({string.Join(',', ids)})";

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            return await conexao.QueryAsync<ComponenteCurricular>(query, commandTimeout: 30000);
        }

        public async Task<IEnumerable<ComponenteCurricularGrupoMatriz>> ListarGruposMatriz()
        {
            var query = @"select id, nome from componente_curricular_grupo_matriz";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryAsync<ComponenteCurricularGrupoMatriz>(query);
        }

        public async Task<IEnumerable<ComponenteCurricularTerritorioSaber>> ObterComponentesTerritorioDosSaberes(string turmaCodigo, IEnumerable<long> componentesCurricularesId)
        {
            var query = ComponenteCurricularConsultas.BuscarTerritorioDoSaber;
            var parametros = new { CodigosComponentesCurriculares = componentesCurricularesId.ToArray(), CodigoTurma = turmaCodigo };

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryAsync<ComponenteCurricularTerritorioSaber>(query, parametros);
            }
        }

        public async Task<IEnumerable<ComponenteCurricular>> ObterComponentesPorTurmaEProfessor(string login, string codigoTurma)
        {
            var query = ComponenteCurricularConsultas.BuscarPorTurmaEProfessor;
            var parametros = new { Login = login, CodigoTurma = codigoTurma };

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryAsync<ComponenteCurricular>(query, parametros);
            }
        }

        public async Task<IEnumerable<ComponenteCurricular>> ListarComponentes()
        {
            var query = @"select cc.id as codigo,
                               cc.descricao_sgp as descricao,
                               cc.eh_territorio as territorioSaber,
                               cc.eh_regencia as ComponentePlanejamentoRegencia,
                               cc.componente_curricular_pai_id as CodComponentePai,
                               cc.grupo_matriz_id as GrupoMatrizId,
                               cc.eh_compartilhada as Compartilhada,
                               cc.permite_lancamento_nota as LancaNota,
                               cc.permite_registro_frequencia as Frequencia,
                               cc.eh_base_nacional as BaseNacional
                          from componente_curricular cc 
                         order by cc.id";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<ComponenteCurricular>(query);
            }
        }

        public async Task<IEnumerable<ComponenteCurricular>> ObterComponentesPorTurmas(string[] codigosTurma)
        {
            var query = @"select distinct iif(pcc.cd_componente_curricular is not null, pcc.cd_componente_curricular,
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

            var parametros = new { codigosTurma };

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            return await conexao.QueryAsync<ComponenteCurricular>(query, parametros);
        }

        public async Task<IEnumerable<ComponenteCurricular>> ObterComponentesPorTurmasEProfessor(string login, string[] codigosTurma)
        {
            var query = @"select distinct iif(pcc.cd_componente_curricular is not null, pcc.cd_componente_curricular,
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

            var parametros = new { Login = login, CodigosTurma = codigosTurma };

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryAsync<ComponenteCurricular>(query, parametros);
            }
        }

        public async Task<IEnumerable<ComponenteCurricularTerritorioSaber>> ObterComponentesTerritorioDosSaberes(string[] turmasCodigo, IEnumerable<long> componentesCurricularesId)
        {
            var query = @"select
						grade_ter.cd_experiencia_pedagogica as CodigoExperienciaPedagogica,
						grade_ter.cd_territorio_saber as CodigoTerritorioSaber,
						ter.dc_territorio_saber as DescricaoTerritorioSaber,
						exp.dc_experiencia_pedagogica as DescricaoExperienciaPedagogica,
						Convert(date, grade_ter.dt_inicio) as DataInicio,
						grade_ter.cd_componente_curricular as CodigoComponenteCurricular,
                        grade_tur.cd_turma_escola as CodigoTurma
					from
						turma_grade_territorio_experiencia grade_ter
						inner join território_saber ter on ter.cd_territorio_saber = grade_ter.cd_territorio_saber
						inner join tipo_experiencia_pedagogica exp on exp.cd_experiencia_pedagogica = grade_ter.cd_experiencia_pedagogica
                        inner join serie_turma_grade grade_tur on grade_tur.cd_serie_grade = grade_ter.cd_serie_grade
					where grade_tur.cd_turma_escola in @codigoTurma and
						grade_ter.cd_componente_curricular in @codigosComponentesCurriculares";

            var parametros = new { CodigosComponentesCurriculares = componentesCurricularesId.ToArray(), CodigoTurma = turmasCodigo };

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryAsync<ComponenteCurricularTerritorioSaber>(query, parametros);
            }
        }

        public async Task<IEnumerable<ComponenteCurricular>> ObterComponentesPorCodigoETurma(string turmaCodigo, long[] componentesCodigo)
        {
            var query = @"select distinct iif(pcc.cd_componente_curricular is not null, pcc.cd_componente_curricular,
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
                    where te.cd_turma_escola = @turmaCodigo ";

            if (componentesCodigo != null && componentesCodigo.Length > 0)
                query = query += $" and pcc.cd_componente_curricular IN ({string.Join(',', componentesCodigo)}) OR cc.cd_componente_curricular IN ({string.Join(',', componentesCodigo)}) ";

            var parametros = new { turmaCodigo };

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            return await conexao.QueryAsync<ComponenteCurricular>(query.ToString(), parametros);
        }

        public async Task<IEnumerable<ComponenteCurricularSondagem>> ObterComponenteCurricularDeSondagemPorId(string componenteCurricularId)
        {
            string query = @"select Id, Descicao, Excluido from ComponenteCurricular where Id = @componenteCurricularId";

            var parametros = new { componenteCurricularId };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);
            return await conexao.QueryAsync<ComponenteCurricularSondagem>(query, parametros);
        }

        public async Task<string> ObterNomeComponenteCurricularPorId(long componenteCurricularId)
        {
            string query = @"select coalesce(descricao_sgp,descricao) as Nome 
                               from Componente_Curricular 
                              where Id = @componenteCurricularId";

            var parametros = new { componenteCurricularId };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryFirstOrDefaultAsync<string>(query, parametros);
        }

        public async Task<IEnumerable<DisciplinaDto>> ObterDisciplinasPorIds(long[] ids)
        {
            var query = $@"select
                                cc.id,
                                cc.id as CodigoComponenteCurricular,
                                cc.area_conhecimento_id as AreaConhecimentoId,
                                cc.componente_curricular_pai_id as CdComponenteCurricularPai,
                                coalesce(cc.descricao_sgp,cc.descricao) as Nome,
                                cc.eh_base_nacional as EhBaseNacional,
                                cc.eh_compartilhada as Compartilhada,
                                cc.eh_regencia as Regencia,
                                cc.eh_territorio as TerritorioSaber,
                                cc.grupo_matriz_id as GrupoMatrizId,
                                ccgm.nome as GrupoMatrizNome,
                                cc.permite_lancamento_nota as LancaNota,
                                cc.permite_registro_frequencia as RegistraFrequencia
                           from componente_curricular cc 
                           left join componente_curricular_grupo_matriz ccgm on ccgm.id = cc.grupo_matriz_id 
                          WHERE cc.id = ANY(@ids)";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<DisciplinaDto>(query, new { ids });
            }
        }

        public async Task<IEnumerable<ComponenteCurricular>> ObterComponentesPorAlunos(int[] alunosCodigos, int anoLetivo, int semestre)
        {
            var query = anoLetivo == DateTime.Today.Year ?
                ComponenteCurricularConsultas.BuscarPorAlunos :
                ComponenteCurricularConsultas.BuscarPorAlunosHistorico;

            var parametros = new
            {
                alunosCodigos,
                anoLetivo,
                semestre
            };

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            return await conexao.QueryAsync<ComponenteCurricular>(query, parametros);
        }
    }
}
