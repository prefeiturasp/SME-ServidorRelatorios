using Dapper;
using Npgsql;
using Org.BouncyCastle.Crypto.Macs;
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
        public readonly static int CODIGORF_LENGTH = 7;
        public readonly static int CODIGOCPF_LENGTH = 11;
        private const long MOTIVO_DISPONIBILIZACAO_ERRO_CADASTRO = 26;

        const string SQL_CAMPOS_TABELA_AGRUPÀMENTO_TERRITORIO = @"codagrupamento as CodigoAgrupamento,
	                                       codterritoriosaber as CodigoTerritorioSaber,
	                                       codexperienciapedagogica as CodigoExperienciaPedagogica,
	                                       dtinicioatribuicao as DtInicioAtribuicao,
                                           anoatribuicao as AnoAtribuicao,
	                                       dtfimatribuicao as DtFimAtribuicao,
                                           dtfimturma as DtFimTurma,
	                                       rfprofessor as RfProfessor,
	                                       codturma as CodigoTurma,
	                                       codcomponentescurriculares as CodigosComponentesCurriculares,
	                                       anoletivo as AnoLetivo,
	                                       codmotivodisponibilizacao as CodigoMotivoDisponibilizacao,
	                                       descterritoriosaber as DescricaoTerritorioSaber,
	                                       descexperienciapedagogica as DescricaoExperienciaPedagogica,
	                                       criado_em as CriadoEm,
                                           alterado_em as AlteradoEm";

        internal static string ObterComponentesCurricularesTerritorioAtribuidos(string condicaoAndWhere)
            => $@"select 	 
                    cc.cd_componente_curricular as CodigoComponenteCurricular,
                    cc.dc_componente_curricular as DescricaoComponenteCurricular,
                    serie_ensino.sg_resumida_serie   as AnoTurma,
                    te.an_letivo as anoletivo,
                    te.cd_turma_escola as TurmaCodigo,
                    vsc.cd_registro_funcional as rfProfessor,
                    tgt.cd_experiencia_pedagogica as CodigoExperienciaPedagogica,
                    tgt.cd_territorio_saber as CodigoTerritorioSaber,
                    ter.dc_territorio_saber as DescricaoTerritorioSaber,
                    exp.dc_experiencia_pedagogica as DescricaoExperienciaPedagogica,
                    aa.dt_atribuicao_aula as dataAtribuicao,
                    aa.an_atribuicao as AnoAtribuicao,
                    te.dt_fim_turma as DataFimTurma,
                    0 as AtribuicaoExterna,
                    max(aa.dt_disponibilizacao_aulas) as dataDisponibilizacao,
                    max(aa.cd_motivo_disponibilizacao) as CodigoMotivoDisponibilizacao
	                    from turma_escola te
                        inner join escola esc ON te.cd_escola = esc.cd_escola
                        inner join v_cadastro_unidade_educacao ue on ue.cd_unidade_educacao = esc.cd_escola
                        inner join unidade_administrativa dre on dre.tp_unidade_administrativa = 24 and ue.cd_unidade_administrativa_referencia = dre.cd_unidade_administrativa
	                    --Serie Ensino
                        inner join serie_turma_escola ON serie_turma_escola.cd_turma_escola = te.cd_turma_escola
                        inner join serie_turma_grade ON serie_turma_grade.cd_turma_escola = serie_turma_escola.cd_turma_escola and serie_turma_grade.dt_fim is null
                        inner join escola_grade ON serie_turma_grade.cd_escola_grade = escola_grade.cd_escola_grade
                        inner join grade ON escola_grade.cd_grade = grade.cd_grade
                        inner join grade_componente_curricular gcc on gcc.cd_grade = grade.cd_grade
                        inner join componente_curricular cc on cc.cd_componente_curricular = gcc.cd_componente_curricular and cc.dt_cancelamento is null
                        inner join serie_ensino ON grade.cd_serie_ensino = serie_ensino.cd_serie_ensino
                        inner join turma_grade_territorio_experiencia tgt on tgt.cd_serie_grade = serie_turma_grade.cd_serie_grade and tgt.cd_componente_curricular = cc.cd_componente_curricular
	                    inner join tipo_experiencia_pedagogica exp on exp.cd_experiencia_pedagogica = tgt.cd_experiencia_pedagogica
	                    inner join território_saber ter on ter.cd_territorio_saber = tgt.cd_territorio_saber
                        -- Atribuição                    
                        inner join atribuicao_aula (nolock) aa on (gcc.cd_grade = aa.cd_grade 
    											                    and gcc.cd_componente_curricular = aa.cd_componente_curricular 
    											                    and aa.cd_serie_grade = serie_turma_grade.cd_serie_grade)
					                                                and aa.dt_cancelamento is null
					                                                and aa.an_atribuicao = te.an_letivo
					                                                AND ( COALESCE(aa.dt_disponibilizacao_aulas, Getdate()) >= '2020-02-05' )	
                                                                    and (aa.cd_motivo_disponibilizacao <> {MOTIVO_DISPONIBILIZACAO_ERRO_CADASTRO} or aa.cd_motivo_disponibilizacao is null)
					                                                and aa.dt_atribuicao_aula <= Getdate()
                        inner join v_cargo_base_cotic (nolock) vcbc on aa.cd_cargo_base_servidor = vcbc.cd_cargo_base_servidor
                        inner join v_servidor_cotic (nolock) vsc on vcbc.cd_servidor = vsc.cd_servidor 
                        where te.st_turma_escola in ('O', 'A', 'C', 'E')
                          {condicaoAndWhere}
                    group by cc.cd_componente_curricular,
                    cc.dc_componente_curricular,
                    serie_ensino.sg_resumida_serie,
                    te.an_letivo,
                    te.cd_turma_escola,
                    vsc.cd_registro_funcional,
                    tgt.cd_experiencia_pedagogica,
                    tgt.cd_territorio_saber,
                    ter.dc_territorio_saber,
                    exp.dc_experiencia_pedagogica,
                    aa.dt_atribuicao_aula,
                    te.dt_fim_turma,
                    aa.an_atribuicao ";

        internal static string ObterComponentesCurricularesTerritorioAtribuidosExterno(string condicaoAndWhere)
            => $@"select 	 
                    cc.cd_componente_curricular as CodigoComponenteCurricular,
                    cc.dc_componente_curricular as DescricaoComponenteCurricular,
                    serie_ensino.sg_resumida_serie   as AnoTurma,
                    te.an_letivo as anoletivo,
                    te.cd_turma_escola as TurmaCodigo,
                    pe.cd_cpf_pessoa as rfProfessor,
                    tgt.cd_experiencia_pedagogica as CodigoExperienciaPedagogica,
                    tgt.cd_territorio_saber as CodigoTerritorioSaber,
                    ter.dc_territorio_saber as DescricaoTerritorioSaber,
                    exp.dc_experiencia_pedagogica as DescricaoExperienciaPedagogica,
                    aa_ext.dt_atribuicao as dataAtribuicao,
                    aa_ext.an_atribuicao as AnoAtribuicao,
                    te.dt_fim_turma as DataFimTurma,
                    1 as AtribuicaoExterna,
                    max(aa_ext.dt_disponibilizacao) as dataDisponibilizacao,
                    max(aa_ext.cd_motivo_disponibilizacao_externo) as CodigoMotivoDisponibilizacao
                        from turma_escola te
                        inner join escola esc ON te.cd_escola = esc.cd_escola
                        inner join v_cadastro_unidade_educacao ue on ue.cd_unidade_educacao = esc.cd_escola
                        inner join unidade_administrativa dre on dre.tp_unidade_administrativa = 24 and ue.cd_unidade_administrativa_referencia = dre.cd_unidade_administrativa
                        --Serie Ensino
                        inner join serie_turma_escola ON serie_turma_escola.cd_turma_escola = te.cd_turma_escola
                        inner join serie_turma_grade ON serie_turma_grade.cd_turma_escola = serie_turma_escola.cd_turma_escola and serie_turma_grade.dt_fim is null
                        inner join escola_grade ON serie_turma_grade.cd_escola_grade = escola_grade.cd_escola_grade
                        inner join grade ON escola_grade.cd_grade = grade.cd_grade
                        inner join grade_componente_curricular gcc on gcc.cd_grade = grade.cd_grade
                        inner join componente_curricular cc on cc.cd_componente_curricular = gcc.cd_componente_curricular and cc.dt_cancelamento is null
                        inner join serie_ensino ON grade.cd_serie_ensino = serie_ensino.cd_serie_ensino
                        inner join turma_grade_territorio_experiencia tgt on tgt.cd_serie_grade = serie_turma_grade.cd_serie_grade and tgt.cd_componente_curricular = cc.cd_componente_curricular
                        inner join tipo_experiencia_pedagogica exp on exp.cd_experiencia_pedagogica = tgt.cd_experiencia_pedagogica
                        inner join território_saber ter on ter.cd_territorio_saber = tgt.cd_territorio_saber
                        -- Atribuição externa
		                inner join atribuicao_externo (nolock) aa_ext
                                   on (gcc.cd_grade = aa_ext.cd_grade and
                                      gcc.cd_componente_curricular = aa_ext.cd_componente_curricular
                                       and aa_ext.cd_serie_grade = serie_turma_grade.cd_serie_grade)
					                   and aa_ext.dt_cancelamento is null
					                   and aa_ext.an_atribuicao = te.an_letivo
                       	                AND ( COALESCE(aa_ext.dt_disponibilizacao, Getdate()) >= '2020-02-05' )	
                                                                    and (aa_ext.cd_motivo_disponibilizacao_externo <> {MOTIVO_DISPONIBILIZACAO_ERRO_CADASTRO} or aa_ext.cd_motivo_disponibilizacao_externo is null)
	                                                                and aa_ext.dt_atribuicao <= Getdate()											
		                inner JOIN contrato_externo ce with (NOLOCK) on ce.cd_contrato_externo = aa_ext.cd_contrato_externo 
  		                inner JOIN pessoa pe with (NOLOCK) on pe.cd_pessoa = ce.cd_pessoa	
	                where te.st_turma_escola in ('O', 'A', 'C', 'E')
                          {condicaoAndWhere}
                    group by cc.cd_componente_curricular,
                    cc.dc_componente_curricular,
                    serie_ensino.sg_resumida_serie,
                    te.an_letivo,
                    te.cd_turma_escola,
                    pe.cd_cpf_pessoa,
                    tgt.cd_experiencia_pedagogica,
                    tgt.cd_territorio_saber,
                    ter.dc_territorio_saber,
                    exp.dc_experiencia_pedagogica,
                    aa_ext.dt_atribuicao,
                    te.dt_fim_turma,
                    aa_ext.an_atribuicao ";

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

        public async Task<IEnumerable<ComponenteCurricularRegenciaApiEol>> ListarRegencia()
        {
            var query = @"SELECT 
                            IdComponenteCurricular
					        ,Turno
					        ,Ano
                        FROM RegenciaComponenteCurricular";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringApiEol);
            return await conexao.QueryAsync<ComponenteCurricularRegenciaApiEol>(query);
        }

        public async Task<IEnumerable<ComponenteCurricularGrupoMatriz>> ListarGruposMatriz()
        {
            var query = @"select id, nome from componente_curricular_grupo_matriz";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryAsync<ComponenteCurricularGrupoMatriz>(query);
        }

        public Task<IEnumerable<ComponenteCurricularTerritorioSaber>> ObterComponentesTerritorioDosSaberes(string turmaCodigo, IEnumerable<long> componentesCurricularesId)
        {
            return ObterComponentesTerritorioDosSaberes(new string[] { turmaCodigo }, componentesCurricularesId);
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

        public async Task<IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO>> ListarInformacoesPedagogicasComponentesCurriculares()
        {
            var query = @"select cc.id as Codigo,
                               cc.componente_curricular_pai_id as CodComponenteCurricularPai,
                                coalesce(cc.descricao_sgp,cc.descricao) as Descricao,
                                cc.descricao_infantil as DescricaoInfantil,
                                cc.eh_base_nacional as BaseNacional,
                                cc.eh_compartilhada as Compartilhada,
                                cc.eh_regencia as EhRegencia,
                                cc.eh_territorio as EhTerritorioSaber,
                                cc.grupo_matriz_id as GrupoMatrizId,
                                ccgm.nome as GrupoMatrizNome,
                                cc.area_conhecimento_id as AreaConhecimentoId,
                                cac.nome as AreaConhecimentoNome,
                                cc.permite_lancamento_nota as LancaNota,
                                cc.permite_registro_frequencia as RegistraFrequencia
                          from componente_curricular cc 
                          left join componente_curricular_grupo_matriz ccgm on ccgm.id = cc.grupo_matriz_id 
                          left join componente_curricular_area_conhecimento cac on cac.id = cc.area_conhecimento_id
                         order by cc.id";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<InformacaoPedagogicaComponenteCurricularSGPDTO>(query);
            }
        }

        public async Task<IEnumerable<ComponenteCurricular>> ObterComponentesPorTurmas(string[] codigosTurma)
        {
            var query = @"select distinct iif(pcc.cd_componente_curricular is not null, pcc.cd_componente_curricular,
                                        cc.cd_componente_curricular) as Codigo,
                                    iif(pcc.dc_componente_curricular is not null, pcc.dc_componente_curricular,
                                        cc.dc_componente_curricular) as Descricao,
                                        esc.tp_escola                    as TipoEscola,
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
                             left join serie_turma_grade ON serie_turma_grade.cd_turma_escola = serie_turma_escola.cd_turma_escola and serie_turma_grade.dt_fim is null
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

        public async Task<IEnumerable<ComponenteCurricular>> ObterComponentesCurricularesPorTurmas(string[] codigosTurma)
        {
            var query = @$"select distinct cc.cd_componente_curricular as Codigo,
                                cc.dc_componente_curricular as Descricao,
                                esc.tp_escola                    as TipoEscola,
                                dtt.qt_hora_duracao              as TurnoTurma,
                                serie_ensino.sg_resumida_serie   as AnoTurma,
                                te.cd_turma_escola               as CodigoTurma  
                            from turma_escola (nolock) te
                            inner join escola (nolock) esc ON te.cd_escola = esc.cd_escola
                            --Serie Ensino
                                inner join serie_turma_escola (nolock) ON serie_turma_escola.cd_turma_escola = te.cd_turma_escola
                                inner join serie_turma_grade (nolock) ON serie_turma_grade.cd_turma_escola = serie_turma_escola.cd_turma_escola
                                inner join escola_grade (nolock) ON serie_turma_grade.cd_escola_grade = escola_grade.cd_escola_grade
                                inner join grade (nolock) ON escola_grade.cd_grade = grade.cd_grade
                                inner join grade_componente_curricular (nolock) gcc on gcc.cd_grade = grade.cd_grade
                                inner join componente_curricular (nolock) cc on cc.cd_componente_curricular = gcc.cd_componente_curricular
                                    and cc.dt_cancelamento is null
                                inner join serie_ensino (nolock)
                                    ON grade.cd_serie_ensino = serie_ensino.cd_serie_ensino                             
                                inner join duracao_tipo_turno dtt on te.cd_tipo_turno = dtt.cd_tipo_turno and te.cd_duracao = dtt.cd_duracao
						where te.cd_turma_escola in @codigosTurma
							and te.st_turma_escola in ('O', 'A', 'C')
                        union all
                        select distinct pcc.cd_componente_curricular as Codigo,
                                        pcc.dc_componente_curricular as Descricao,
                                        esc.tp_escola                    as TipoEscola,
                                        dtt.qt_hora_duracao              as TurnoTurma,
                                        ''   as AnoTurma,
                                        te.cd_turma_escola               as CodigoTurma  
                        from turma_escola (nolock) te
                            inner join escola (nolock) esc ON te.cd_escola = esc.cd_escola
                        --Programa
                            inner join turma_escola_grade_programa (nolock) tegp on tegp.cd_turma_escola = te.cd_turma_escola
                            inner join escola_grade (nolock) teg on teg.cd_escola_grade = tegp.cd_escola_grade
                            inner join grade (nolock) pg on pg.cd_grade = teg.cd_grade
                            inner join grade_componente_curricular (nolock) pgcc on pgcc.cd_grade = teg.cd_grade
                            inner join componente_curricular (nolock) pcc on pgcc.cd_componente_curricular = pcc.cd_componente_curricular and pcc.dt_cancelamento is null                        
                            inner join duracao_tipo_turno dtt on te.cd_tipo_turno = dtt.cd_tipo_turno and te.cd_duracao = dtt.cd_duracao
                        where te.cd_turma_escola in @codigosTurma
                            and te.st_turma_escola in ('O', 'A', 'C') ";                         

            var parametros = new { CodigosTurma = Array.ConvertAll(codigosTurma, codigo => int.Parse(codigo)) };

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryAsync<ComponenteCurricular>(query, parametros,commandTimeout:180);
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

        

        public async Task<IEnumerable<ComponenteCurricular>> ObterComponentesPorAlunos(int[] codigosTurmas, int[] alunosCodigos, int anoLetivo, int semestre, bool consideraHistorico = false)
        {
            var query = !consideraHistorico ?
            ComponenteCurricularConsultas.BuscarPorAlunos :
            ComponenteCurricularConsultas.BuscarPorAlunosHistorico;

            query += " and te.cd_turma_escola in @codigosTurmas ";
            query += " order by 2";

            var parametros = new
            {
                alunosCodigos,
                anoLetivo,
                semestre,
                codigosTurmas
            };

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            var resultado = await conexao.QueryAsync<ComponenteCurricular>(query, parametros);

            if (!consideraHistorico && !resultado.Any())
                resultado = await conexao.QueryAsync<ComponenteCurricular>(ComponenteCurricularConsultas.BuscarPorAlunosHistorico, parametros);

            return resultado;
        }

        public async Task<long> ObterGrupoMatrizIdPorComponenteCurricularId(long componenteCurricularId)
        {
            string sql = "select grupo_matriz_id from componente_curricular where id = @componenteCurricularId";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<long>(sql, new { componenteCurricularId});
            }
        }

        public async Task<bool> VerificaSeComponenteEhTerritorio(long componenteCurricularId)
        {
            string sql = "select eh_territorio from componente_curricular where id = @componenteCurricularId";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<bool>(sql, new { componenteCurricularId });
            }
        }

        public async Task<IEnumerable<AgrupamentoAtribuicaoTerritorioSaber>> ObterAgrupamentosTerritorioSaber(long[] ids)
        {
            var sql = $@"select {SQL_CAMPOS_TABELA_AGRUPÀMENTO_TERRITORIO} from agrupamentoatribuicaoterritoriosaber 
                         where codagrupamento = ANY(@ids) ";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringApiEol))
            {
                return await conexao.QueryAsync<AgrupamentoAtribuicaoTerritorioSaber>(sql, new { ids });
            }
        }

        public async Task<IEnumerable<AgrupamentoAtribuicaoTerritorioSaber>> ObterAgrupamentosTerritorioSaber(long codigoTurma,
                                                                                                 long? codigoTerritorioSaber,
                                                                                                 long? codigoExperienciaPegagogica,
                                                                                                 long? codigoComponenteCurricular = null,
                                                                                                 DateTime? dataBase = null,
                                                                                                 DateTime? dataInicio = null,
                                                                                                 string rfProfessor = null,
                                                                                                 string codigosComponentesCurriculares = null,
                                                                                                 bool? encerramentoAtribuicaoViaAtualizacaoComponentesAgrupados = null)
        {
            var sql = $@"select {SQL_CAMPOS_TABELA_AGRUPÀMENTO_TERRITORIO} from agrupamentoatribuicaoterritoriosaber 
                         where  codturma = @codigoTurma 
                                {(codigoTerritorioSaber != null ? " and codterritoriosaber = @codigoTerritorioSaber " : string.Empty)}
                                {(codigoExperienciaPegagogica != null ? " and codexperienciapedagogica = @codigoExperienciaPegagogica " : string.Empty)}
                                {(codigoComponenteCurricular != null ? " and @CodigoComponenteCurricular = ANY(string_to_array(codcomponentescurriculares, ',')) " : string.Empty)}
                                {(codigosComponentesCurriculares != null ? " and codcomponentescurriculares = @codigosComponentesCurriculares " : string.Empty)}
                                {(!string.IsNullOrEmpty(rfProfessor) ? " and rfprofessor = @rfProfessor " : string.Empty)}
                                {(dataInicio != null ? " and dtinicioatribuicao = @dataInicio " : string.Empty)}
                                {(encerramentoAtribuicaoViaAtualizacaoComponentesAgrupados != null ? " and encerramento_atribuicao_agrupamento_atualizado = @encerramentoAtribuicaoViaAtualizacaoComponentesAgrupados " : string.Empty)}
                                and dtinicioatribuicao <= @dataBase ";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringApiEol))
            {
                return await conexao.QueryAsync<AgrupamentoAtribuicaoTerritorioSaber>(sql, new
                {
                    codigoTurma,
                    codigoTerritorioSaber,
                    codigoExperienciaPegagogica,
                    codigoComponenteCurricular = codigoComponenteCurricular.ToString(),
                    dataBase = dataBase ?? DateTime.Today,
                    rfProfessor,
                    codigosComponentesCurriculares,
                    dataInicio,
                    encerramentoAtribuicaoViaAtualizacaoComponentesAgrupados
                });
            }
        }

        public async Task<IEnumerable<ComponenteCurricularTerritorioAtribuidoTurmaDTO>> ObterComponentesCurricularesTerritorioAtribuidos(long codigoTurma, string rfProf = null)
        {
            var condicaoAndWhere = $@" and te.cd_turma_escola = @CodigoTurma 
                                          {(!string.IsNullOrEmpty(rfProf) ? " and vsc.cd_registro_funcional = @rfProf " : string.Empty)}";

            var condicaoAndWhereAtribuicaoExterno = $@" and te.cd_turma_escola = @CodigoTurma 
                                          {(!string.IsNullOrEmpty(rfProf) ? " and pe.cd_cpf_pessoa = @cpfProf " : string.Empty)}";

            return await ObterComponentesCurricularesTerritorioAtribuidos(condicaoAndWhere, condicaoAndWhereAtribuicaoExterno, new
            {
                codigoTurma,
                rfProf = new DbString { Value = rfProf, Length = CODIGORF_LENGTH, IsFixedLength = true, IsAnsi = true },
                cpfProf = new DbString { Value = rfProf, Length = CODIGOCPF_LENGTH, IsFixedLength = true, IsAnsi = true }
            });
        }

        private async Task<IEnumerable<ComponenteCurricularTerritorioAtribuidoTurmaDTO>> ObterComponentesCurricularesTerritorioAtribuidos(string whereAtribuicaoAula, string whereAtribuicaoExterna, object parametro)
        {
            var consultaSQL = $@"{ObterComponentesCurricularesTerritorioAtribuidos(whereAtribuicaoAula)}
                                 union all   
                                 {ObterComponentesCurricularesTerritorioAtribuidosExterno(whereAtribuicaoExterna)}
                                 order by dataAtribuicao, CodigoTerritorioSaber, CodigoExperienciaPedagogica, rfProfessor, CodigoComponenteCurricular;";
            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryAsync<ComponenteCurricularTerritorioAtribuidoTurmaDTO>(consultaSQL, parametro);
            }
        }

    }
}
