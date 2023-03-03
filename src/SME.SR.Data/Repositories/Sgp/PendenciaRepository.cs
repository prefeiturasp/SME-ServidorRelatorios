using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class PendenciaRepository : IPendenciaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public PendenciaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<RelatorioPendenciasQueryRetornoDto>> ObterPendencias(int anoLetivo, string dreCodigo, string ueCodigo, long modalidadeId, int? semestre,
                                                            string[] turmasCodigo, long[] componentesCodigo, int bimestre, bool pendenciaResolvida, int[] tipoPendenciaGrupo, string usuarioRf, bool exibirHistorico)
        {
            StringBuilder query = new StringBuilder();
            if (tipoPendenciaGrupo.Count() == 1)
            {
                int pendencia = tipoPendenciaGrupo[0];
                if (pendencia == (int)TipoPendenciaGrupo.Calendario)
                {
                    query.AppendLine(ObterPendenciasCalendario(modalidadeId, semestre, turmasCodigo, componentesCodigo, bimestre, usuarioRf, exibirHistorico));
                }
                if (pendencia == (int)TipoPendenciaGrupo.Fechamento)
                {
                    query.AppendLine(ObterPendenciasFechamento(modalidadeId, semestre, turmasCodigo, componentesCodigo, bimestre, pendenciaResolvida, usuarioRf, exibirHistorico));
                }
                if (pendencia == (int)TipoPendenciaGrupo.AEE)
                {
                    query.AppendLine(ObterPendenciasAee(modalidadeId, semestre, turmasCodigo, componentesCodigo, bimestre, usuarioRf, exibirHistorico));

                }
                if (pendencia == (int)TipoPendenciaGrupo.DiarioClasse)
                {
                    query.AppendLine(ObterPendenciasDiarioClasse(modalidadeId, semestre, turmasCodigo, componentesCodigo, bimestre, usuarioRf, exibirHistorico));
                }
                if (pendencia == (int)TipoPendenciaGrupo.Todos)
                {
                    query.AppendLine(ObterPendenciasCalendario(modalidadeId, semestre, turmasCodigo, componentesCodigo, bimestre, usuarioRf, exibirHistorico));
                    query.AppendLine(" union all ");
                    query.AppendLine(ObterPendenciasFechamento(modalidadeId, semestre, turmasCodigo, componentesCodigo, bimestre, pendenciaResolvida, usuarioRf, exibirHistorico));
                    query.AppendLine(" union all ");
                    query.AppendLine(ObterPendenciasAee(modalidadeId, semestre, turmasCodigo, componentesCodigo, bimestre, usuarioRf, exibirHistorico));
                    query.AppendLine(" union all ");
                    query.AppendLine(ObterPendenciasDiarioClasse(modalidadeId, semestre, turmasCodigo, componentesCodigo, bimestre, usuarioRf, exibirHistorico));
                }
            }
            else
            {
                if (tipoPendenciaGrupo.Count() > 1)
                {
                    int volta = 0;
                    for (int i = 0; i < tipoPendenciaGrupo.Count(); i++)
                    {
                        var pendencia = tipoPendenciaGrupo[i];

                        if (pendencia == (int)TipoPendenciaGrupo.Calendario)
                        {
                            query.AppendLine(ObterPendenciasCalendario(modalidadeId, semestre, turmasCodigo, componentesCodigo, bimestre, usuarioRf, exibirHistorico));
                        }
                        if (pendencia == (int)TipoPendenciaGrupo.Fechamento)
                        {
                            query.AppendLine(ObterPendenciasFechamento(modalidadeId, semestre, turmasCodigo, componentesCodigo, bimestre, pendenciaResolvida, usuarioRf, exibirHistorico));
                        }
                        if (pendencia == (int)TipoPendenciaGrupo.AEE)
                        {
                            query.AppendLine(ObterPendenciasAee(modalidadeId, semestre, turmasCodigo, componentesCodigo, bimestre, usuarioRf, exibirHistorico));

                        }
                        if (pendencia == (int)TipoPendenciaGrupo.DiarioClasse)
                        {
                            query.AppendLine(ObterPendenciasDiarioClasse(modalidadeId, semestre, turmasCodigo, componentesCodigo, bimestre, usuarioRf, exibirHistorico));
                        }
                        if (pendencia == (int)TipoPendenciaGrupo.Todos)
                        {
                            query.AppendLine(ObterPendenciasCalendario(modalidadeId, semestre, turmasCodigo, componentesCodigo, bimestre, usuarioRf, exibirHistorico));
                            query.AppendLine(" union all ");
                            query.AppendLine(ObterPendenciasFechamento(modalidadeId, semestre, turmasCodigo, componentesCodigo, bimestre, pendenciaResolvida, usuarioRf, exibirHistorico));
                            query.AppendLine(" union all ");
                            query.AppendLine(ObterPendenciasAee(modalidadeId, semestre, turmasCodigo, componentesCodigo, bimestre, usuarioRf, exibirHistorico));
                            query.AppendLine(" union all ");
                            query.AppendLine(ObterPendenciasDiarioClasse(modalidadeId, semestre, turmasCodigo, componentesCodigo, bimestre, usuarioRf, exibirHistorico));
                        }
                        volta++;
                        if (volta < tipoPendenciaGrupo.Count())
                            query.AppendLine(" union all ");
                    }
                }
            }
            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            var pendencias = new Dictionary<string, RelatorioPendenciasQueryRetornoDto>();

            await conexao.QueryAsync<RelatorioPendenciasQueryRetornoDto, string, RelatorioPendenciasQueryRetornoDto>(query.ToString()
                    , (dto, detalhe) =>
                    {
                        var pendencia = new RelatorioPendenciasQueryRetornoDto();

                        if (!pendencias.TryGetValue(dto.PendenciaId, out pendencia))
                        {
                            dto.AdicionaDetalhe(detalhe);
                            pendencias.Add(dto.PendenciaId, dto);
                        }
                        else
                            pendencia.AdicionaDetalhe(detalhe);

                        return dto;
                    }
                    , new { anoLetivo, dreCodigo, ueCodigo, modalidadeId, semestre, turmasCodigo, componentesCodigo, bimestre, pendenciaResolvida, usuarioRf }
                    , splitOn: "pendenciaId, Detalhe");

            return pendencias.Values
                .OrderBy(x => x.Criador)
                .ThenBy(x => x.TipoPendencia);
        }


        private string ObterPendenciasCalendario(long modalidadeId, int? semestre, string[] turmasCodigo, long[] componentesCodigo, int bimestre, string usuarioRf, bool exibirHistorico)
        {
            var query = new StringBuilder(@$"select distinct 
	                        p.id || usu.rf_codigo as pendenciaId,
                            p.titulo,
	                        p.descricao as Descricao,
	                        p.situacao,
                            p.instrucao, 
	                        d.abreviacao as DreNome,
	                        te.descricao || ' - ' || u.nome as UeNome,
	                        0 as AnoLetivo,
	                        0 as ModalidadeCodigo,
	                        0 as semestre,
	                        '' as TurmaNome,
                            '' as TurmaCodigo,
	                        0 as DisciplinaId,
	                        pe.bimestre,
                            usu.nome as criador,
                            usu.login as criadorRf,
	                        p.alterado_por as aprovador,
	                        p.alterado_rf as aprovadorRf,
                            'Calendário' as TipoPendencia,
                            true as OutrasPendencias,
                            p.tipo,
                            '' as Detalhe
                        from pendencia_calendario_ue pcu 
                        inner join pendencia p 
	                        on pcu.pendencia_id  = p.id
	                    inner join ue u 
	                        on pcu.ue_id  = u.id    
	                    inner join tipo_escola te
                            on te.cod_tipo_escola_eol = u.tipo_escola   
	                    inner join dre d 
	                        on u.dre_id  = d.id 
	                    inner join tipo_calendario tc
		                    on pcu.tipo_calendario_id  = tc.id 
	                    inner join periodo_escolar pe
		                    on pe.tipo_calendario_id = tc.id 
                        inner join pendencia_usuario pu 
                              on pu.pendencia_id = p.id
                        inner join usuario usu 
                              on usu.id = pu.usuario_id
                        where tc.ano_letivo = @anoLetivo
                        and d.dre_id  = @dreCodigo
                        and u.ue_id  = @ueCodigo
                        and p.situacao in(1,2)
                        and p.tipo = 12
                        and not p.excluido ");

            if (!String.IsNullOrEmpty(usuarioRf) && usuarioRf.Length > 0)
                query.AppendLine(" and usu.login = @usuarioRf ");

            if (bimestre > 0)
                query.AppendLine(" and pe.bimestre = @bimestre ");

            query.AppendLine(" union all  ");

            query.AppendLine(@"select distinct 
                            p.id || usu.rf_codigo as pendenciaId,
                            p.titulo,
	                        p.descricao as Descricao,
	                        p.situacao,
                            p.instrucao, 
	                        d.abreviacao as DreNome,
	                        te.descricao || ' - ' || u.nome as UeNome,
	                        0 as AnoLetivo,
	                        0 as ModalidadeCodigo,
	                        0 as semestre,
	                        '' as TurmaNome,
                            '' as TurmaCodigo,
	                        0 as DisciplinaId,
	                        pe.bimestre,
                            usu.nome as criador,
                            usu.login as criadorRf,
	                        p.alterado_por as aprovador,
	                        p.alterado_rf as aprovadorRf,
                            'Calendário' as TipoPendencia,
                            true as OutrasPendencias,
                            p.tipo,
                            ps.descricao ||' ('|| ppe.quantidade_eventos ||')' as Detalhe
                         from pendencia_calendario_ue pcu 
                        inner join pendencia_parametro_evento ppe 
                            on ppe.pendencia_calendario_ue_id = pcu.id
                        inner join parametros_sistema ps 
                            on ps.id = ppe.parametro_sistema_id 
                        inner join pendencia p 
	                        on pcu.pendencia_id  = p.id
	                    inner join ue u 
	                        on pcu.ue_id  = u.id    
	                    inner join tipo_escola te
                            on te.cod_tipo_escola_eol = u.tipo_escola   
	                    inner join dre d 
	                        on u.dre_id  = d.id 
	                    inner join tipo_calendario tc
		                    on pcu.tipo_calendario_id  = tc.id 
                        inner join periodo_escolar pe
		                    on pe.tipo_calendario_id = tc.id 
                        inner join pendencia_usuario pu 
                              on pu.pendencia_id = p.id
                        inner join usuario usu 
                              on usu.id = pu.usuario_id
                        where tc.ano_letivo = @anoLetivo
                        and d.dre_id  = @dreCodigo
                        and u.ue_id  = @ueCodigo
                        and p.situacao in (1,2)
                        and not p.excluido");

            if (!String.IsNullOrEmpty(usuarioRf) && usuarioRf.Length > 0)
                query.AppendLine(" and usu.login = @usuarioRf ");

            if (bimestre > 0)
                query.AppendLine(" and pe.bimestre = @bimestre ");

            return query.ToString();
        }

        private string ObterPendenciasFechamento(long modalidadeId, int? semestre, string[] turmasCodigo, long[] componentesCodigo, int bimestre, bool pendenciaResolvida, string usuarioRf, bool exibirHistorico)
        {
            var query = new StringBuilder(@$"select distinct
                            p.id || usu.rf_codigo as pendenciaId,
                            p.titulo,
	                        p.descricao as Descricao,
	                        p.situacao,
                            p.instrucao,
	                        d.abreviacao as DreNome,
	                        te.descricao || ' - ' || u.nome as UeNome,
	                        t.ano_letivo as AnoLetivo,
	                        t.modalidade_codigo as ModalidadeCodigo,
	                        t.semestre,
                            t.nome ||
	                            case t.ano
		                            when '0' then ''
		                            else ' - '|| t.ano || 'ºANO'
	                            end as TurmaNome,
                            t.turma_id as TurmaCodigo,
	                        ftd.disciplina_id as DisciplinaId,
	                        pe.bimestre,
                            usu.nome as criador,
                            usu.login as criadorRf,
	                        p.alterado_por as aprovador,
	                        p.alterado_rf as aprovadorRf,
                            'Fechamento' as TipoPendencia,
                            false as OutrasPendencias,
                            p.tipo,
                            '' as Detalhe
                        from pendencia_fechamento pf
                        inner join pendencia p 
	                        on pf.pendencia_id  = p.id
                        inner join fechamento_turma_disciplina ftd 
	                        on ftd.id = pf.fechamento_turma_disciplina_id 
                        inner join fechamento_turma ft
	                        on ftd.fechamento_turma_id = ft.id 
                        inner join turma t 
	                        on t.id = ft.turma_id 	
                        inner join ue u 
	                        on t.ue_id  = u.id 
                        inner join dre d 
	                        on u.dre_id  = d.id 
                        inner join periodo_escolar pe 
	                        on ft.periodo_escolar_id  = pe.id 
                        inner join tipo_escola te
                            on te.cod_tipo_escola_eol = u.tipo_escola
                        inner join pendencia_usuario pu 
                              on pu.pendencia_id = p.id
                        inner join usuario usu 
                              on usu.id = pu.usuario_id 
                        where t.ano_letivo = @anoLetivo
                        and d.dre_id  = @dreCodigo
                        and u.ue_id  = @ueCodigo
                        and not p.excluido ");

            if (modalidadeId > 0)
                query.AppendLine(" and t.modalidade_codigo = @modalidadeId");

            if (!String.IsNullOrEmpty(usuarioRf) && usuarioRf.Length > 0)
                query.AppendLine(" and usu.login = @usuarioRf ");

            if (pendenciaResolvida)
                query.AppendLine(" and p.situacao in(1,2,3) ");
            else
                query.AppendLine(" and p.situacao in(1,2) ");

            if (exibirHistorico)
                query.AppendLine(" and t.historica  = true ");

            if (semestre.HasValue)
                query.AppendLine($" and t.semestre = @semestre ");

            if (turmasCodigo != null && turmasCodigo.Any(t => t != "-99" && t != null))
                query.AppendLine($" and t.turma_id = any(@turmasCodigo) ");

            if (componentesCodigo != null && componentesCodigo.Any(t => t != -99))
                query.AppendLine($" and ftd.disciplina_id = any(@componentesCodigo)");

            if (bimestre > 0)
                query.AppendLine($" and pe.bimestre  = @bimestre");

            query.AppendLine(" union all");
            query.AppendLine(@"select distinct
                            p.id || usu.rf_codigo as pendenciaId,
                            p.titulo,
	                        p.descricao as Descricao,
	                        p.situacao,
                            p.instrucao,
	                        d.abreviacao as DreNome,
	                        te.descricao || ' - ' || ue.nome as UeNome,
	                        t.ano_letivo as AnoLetivo,
	                        t.modalidade_codigo as ModalidadeCodigo,
	                        t.semestre,
                            t.nome ||
	                            case t.ano
		                            when '0' then ''
		                            else ' - '|| t.ano || 'ºANO'
	                            end as TurmaNome,
                            t.turma_id as TurmaCodigo,
	                        pp.componente_curricular_id as DisciplinaId,
	                        pe.bimestre,
                            usu.nome as criador,
                            usu.login as criadorRf,
	                        p.alterado_por as aprovador,
	                        p.alterado_rf as aprovadorRf,
                            'Fechamento' as TipoPendencia,
                            false as OutrasPendencias,
                            p.tipo,
                            coalesce(cc.descricao_sgp, cc.descricao) ||' - '|| coalesce(u.nome, '') ||'('|| u.rf_codigo ||')' as Detalhe
                        from pendencia_professor pp
                        inner join componente_curricular cc on cc.id = pp.componente_curricular_id
                        inner join usuario u on u.rf_codigo = pp.professor_rf
                        inner join pendencia p 
	                        on pp.pendencia_id  = p.id
                        inner join turma t 
	                        on t.id = pp.turma_id 	
                        inner join ue  
	                        on t.ue_id  = ue.id 
                        inner join dre d 
	                        on ue.dre_id  = d.id 
                        inner join periodo_escolar pe 
	                        on pp.periodo_escolar_id  = pe.id 
                        inner join tipo_escola te
                            on te.cod_tipo_escola_eol = ue.tipo_escola
                        inner join pendencia_usuario pu 
                              on pu.pendencia_id = p.id
                        inner join usuario usu 
                              on usu.id = pu.usuario_id 
                        where t.ano_letivo = @anoLetivo
                        and p.tipo = 15
                        and d.dre_id  = @dreCodigo
                        and ue.ue_id  = @ueCodigo
                        and not p.excluido ");

            if (modalidadeId > 0)
                query.AppendLine(" and t.modalidade_codigo = @modalidadeId");

            if (!String.IsNullOrEmpty(usuarioRf) && usuarioRf.Length > 0)
                query.AppendLine(" and usu.login = @usuarioRf ");

            if (pendenciaResolvida)
                query.AppendLine(" and p.situacao in (1,2,3) ");
            else
                query.AppendLine(" and p.situacao in (1,2) ");

            if (exibirHistorico)
                query.AppendLine(" and t.historica  = true ");

            if (semestre.HasValue)
                query.AppendLine($" and t.semestre = @semestre ");

            if (turmasCodigo != null && turmasCodigo.Any(t => t != "-99" && t != null))
                query.AppendLine($" and t.turma_id = any(@turmasCodigo) ");

            if (componentesCodigo != null && componentesCodigo.Any(t => t != -99))
                query.AppendLine($" and pp.componente_curricular_id = any(@componentesCodigo)");

            if (bimestre > 0)
                query.AppendLine($" and pe.bimestre  = @bimestre");

            query.AppendLine(" union all");
            query.AppendLine(@"select distinct
                            p.id || usu.rf_codigo as pendenciaId,
                            p.titulo,
	                        p.descricao as Descricao,
	                        p.situacao,
                            p.instrucao,
	                        d.abreviacao as DreNome,
	                        te.descricao || ' - ' || ue.nome as UeNome,
	                        t.ano_letivo as AnoLetivo,
	                        t.modalidade_codigo as ModalidadeCodigo,
	                        t.semestre,
                            t.nome ||
	                            case t.ano
		                            when '0' then ''
		                            else ' - '|| t.ano || 'ºANO'
	                            end as TurmaNome,
                            t.turma_id as TurmaCodigo,
	                        pp.componente_curricular_id as DisciplinaId,
	                        pe.bimestre,
                            usu.nome as criador,
                            usu.login as criadorRf,
	                        p.alterado_por as aprovador,
	                        p.alterado_rf as aprovadorRf,
                            'Fechamento' as TipoPendencia,
                            false as OutrasPendencias,
                            p.tipo,
                            '' as Detalhe
                        from pendencia_professor pp
                        inner join componente_curricular cc on cc.id = pp.componente_curricular_id
                        inner join usuario u on u.rf_codigo = pp.professor_rf
                        inner join pendencia p 
	                        on pp.pendencia_id  = p.id
                        inner join turma t 
	                        on t.id = pp.turma_id 	
                        inner join ue  
	                        on t.ue_id  = ue.id 
                        inner join dre d 
	                        on ue.dre_id  = d.id 
                        inner join periodo_escolar pe 
	                        on pp.periodo_escolar_id  = pe.id 
                        inner join tipo_escola te
                            on te.cod_tipo_escola_eol = ue.tipo_escola
                        inner join pendencia_usuario pu 
                              on pu.pendencia_id = p.id
                        inner join usuario usu 
                              on usu.id = pu.usuario_id 
                        where t.ano_letivo = @anoLetivo
                        and p.tipo <> 15
                        and d.dre_id  = @dreCodigo
                        and ue.ue_id  = @ueCodigo
                        and not p.excluido ");

            if (modalidadeId > 0)
                query.AppendLine(" and t.modalidade_codigo = @modalidadeId");

            if (!String.IsNullOrEmpty(usuarioRf) && usuarioRf.Length > 0)
                query.AppendLine(" and usu.login = @usuarioRf ");

            if (pendenciaResolvida)
                query.AppendLine(" and p.situacao in (1,2,3) ");
            else
                query.AppendLine(" and p.situacao in (1,2) ");

            if (exibirHistorico)
                query.AppendLine(" and t.historica  = true ");

            if (semestre.HasValue)
                query.AppendLine($" and t.semestre = @semestre ");

            if (turmasCodigo != null && turmasCodigo.Any(t => t != "-99" && t != null))
                query.AppendLine($" and t.turma_id = any(@turmasCodigo) ");

            if (componentesCodigo != null && componentesCodigo.Any(t => t != -99))
                query.AppendLine($" and pp.componente_curricular_id = any(@componentesCodigo)");

            if (bimestre > 0)
                query.AppendLine($" and pe.bimestre  = @bimestre");

            return query.ToString();

        }

        private string ObterPendenciasAee(long modalidadeId, int? semestre, string[] turmasCodigo, long[] componentesCodigo, int bimestre, string usuarioRf, bool exibirHistorico)
        {
            var outrasPendencias = new StringBuilder();
            var query = new StringBuilder($@" select distinct 
                            p.id || usu.rf_codigo as pendenciaId,
                            p.titulo,
	                        p.descricao as Descricao,
	                        p.situacao,
                            p.instrucao,
                            d.abreviacao as DreNome,
                            te.descricao || ' - ' || u.nome as UeNome,
                            t.ano_letivo as AnoLetivo,
                            t.modalidade_codigo as ModalidadeCodigo,
                            t.semestre,
                            t.nome ||
	                            case t.ano
		                            when '0' then ''
		                            else ' - '|| t.ano || 'ºANO'
	                            end as TurmaNome,
                            t.turma_id as TurmaCodigo,
                            0 as DisciplinaId,
                            0 as bimestre,
                            usu.nome as criador,
                            usu.login as criadorRf,
                            p.alterado_por as aprovador,
                            p.alterado_rf as aprovadorRf,
                            'AEE' as TipoPendencia,
                            true as OutrasPendencias,
                            p.tipo,
                            '' as Detalhe
                        from pendencia_plano_aee ppa
                        inner join pendencia p 
                            on p.id = ppa.pendencia_id
                        inner join plano_aee pa 
                            on pa.id = ppa.plano_aee_id 
                        inner join turma t 
                            on t.id = pa.turma_id 
                        inner join ue u 
                            on u.id  = t.ue_id       
                        inner join tipo_escola te
                            on te.cod_tipo_escola_eol = u.tipo_escola   
                        inner join dre d 
                            on u.dre_id  = d.id 
                        inner join pendencia_usuario pu 
                              on pu.pendencia_id = p.id
                        inner join usuario usu 
                              on usu.id = pu.usuario_id 
                        where t.ano_letivo = @anoLetivo
                        and d.dre_id  = @dreCodigo
                        and u.ue_id  = @ueCodigo
                        and p.situacao in (1,2)
                        and not p.excluido ");

            if (modalidadeId > 0)
                query.AppendLine(" and t.modalidade_codigo = @modalidadeId");

            if (!String.IsNullOrEmpty(usuarioRf) && usuarioRf.Length > 0)
                query.AppendLine(" and usu.login = @usuarioRf ");

            if (exibirHistorico)
                query.AppendLine(" and t.historica  = true ");

            if (semestre.HasValue)
                query.AppendLine($" and t.semestre = @semestre ");

            if (turmasCodigo != null && turmasCodigo.Any(t => t != "-99" && t != null))
                query.AppendLine($" and t.turma_id = any(@turmasCodigo) ");

            query.AppendLine("union all");

            query.AppendLine(@" select distinct 
                            p.id || usu.rf_codigo as pendenciaId,
                            p.titulo,
	                        p.descricao as Descricao,
	                        p.situacao,
                            p.instrucao,
                            d.abreviacao as DreNome,
                            te.descricao || ' - ' || u.nome as UeNome,
                            t.ano_letivo as AnoLetivo,
                            t.modalidade_codigo as ModalidadeCodigo,
                            t.semestre,
                            t.nome ||
	                            case t.ano
		                            when '0' then ''
		                            else ' - '|| t.ano || 'ºANO'
	                            end as TurmaNome,
                            t.turma_id as TurmaCodigo,
                            0 as DisciplinaId,
                            0 as bimestre,
                            usu.nome as criador,
                            usu.login as criadorRf,
                            p.alterado_por as aprovador,
                            p.alterado_rf as aprovadorRf,
                            'AEE' as TipoPendencia,
                            true as OutrasPendencias,
                            p.tipo,
                            '' as Detalhe
                        from pendencia_encaminhamento_aee pea
                        inner join pendencia p 
                            on p.id = pea.pendencia_id
                        inner join encaminhamento_aee ea  
                            on ea.id = pea.encaminhamento_aee_id 
                        inner join turma t 
                            on t.id = ea.turma_id 
                        inner join ue u 
                            on u.id  = t.ue_id       
                        inner join tipo_escola te
                            on te.cod_tipo_escola_eol = u.tipo_escola   
                        inner join dre d 
                            on u.dre_id  = d.id
                        inner join pendencia_usuario pu 
                              on pu.pendencia_id = p.id
                        inner join usuario usu 
                              on usu.id = pu.usuario_id 
                        where t.ano_letivo = @anoLetivo
                        and d.dre_id  = @dreCodigo
                        and u.ue_id  = @ueCodigo
                        and p.situacao in(1,2)
                        and not p.excluido ");

            if (modalidadeId > 0)
                query.AppendLine(" and t.modalidade_codigo = @modalidadeId");

            if (!String.IsNullOrEmpty(usuarioRf) && usuarioRf.Length > 0)
                query.AppendLine(" and usu.login = @usuarioRf ");

            if (exibirHistorico)
                query.AppendLine(" and t.historica  = true ");

            if (semestre.HasValue)
                query.AppendLine($" and t.semestre = @semestre ");

            if (turmasCodigo != null && turmasCodigo.Any(t => t != "-99" && t != null))
                query.AppendLine($" and t.turma_id = any(@turmasCodigo) ");

            return query.ToString();
        }

        private string ObterPendenciasDiarioClasse(long modalidadeId, int? semestre, string[] turmasCodigo, long[] componentesCodigo, int bimestre, string usuarioRf, bool exibirHistorico)
        {
            var query = new StringBuilder($@"select distinct 
                            p.id || usu.rf_codigo as pendenciaId,
                            p.titulo,
	                        p.descricao as Descricao,
	                        p.situacao,
                            p.instrucao,
                            d.abreviacao as DreNome,
                            te.descricao || ' - ' || u.nome as UeNome,
                            t.ano_letivo as AnoLetivo,
                            t.modalidade_codigo as ModalidadeCodigo,
                            t.semestre,
                            t.nome ||
	                            case t.ano
		                            when '0' then ''
		                            else ' - '|| t.ano || 'ºANO'
	                            end as TurmaNome,
                            t.turma_id as TurmaCodigo,
                            0 as DisciplinaId,
                            0 as bimestre,
                            usu.nome as criador,
                            usu.login as criadorRf,
                            p.alterado_por as aprovador,
                            p.alterado_rf as aprovadorRf,
                            'Diário de Classe' as TipoPendencia,
                            true as OutrasPendencias,
                            p.tipo,
                            cast(pria.codigo_aluno as varchar) as Detalhe
                        from pendencia_registro_individual pri 
                        inner join pendencia_registro_individual_aluno pria 
	                        on pri.id = pria.pendencia_registro_individual_id
                        inner join pendencia p 
                            on p.id = pri.pendencia_id
                        inner join turma t 
                            on t.id = pri.turma_id 
                        inner join ue u 
                            on u.id  = t.ue_id       
                        inner join tipo_escola te
                            on te.cod_tipo_escola_eol = u.tipo_escola   
                        inner join dre d 
                            on u.dre_id  = d.id
                        inner join pendencia_usuario pu 
                              on pu.pendencia_id = p.id
                        inner join usuario usu 
                              on usu.id = pu.usuario_id 
                        where t.ano_letivo = @anoLetivo
                        and d.dre_id  = @dreCodigo
                        and u.ue_id  = @ueCodigo
                        and p.situacao in (1,2)
                        and not p.excluido ");

            if (modalidadeId > 0)
                query.AppendLine(" and t.modalidade_codigo = @modalidadeId");

            if (!String.IsNullOrEmpty(usuarioRf) && usuarioRf.Length > 0)
                query.AppendLine(" and usu.login = @usuarioRf ");

            if (exibirHistorico)
                query.AppendLine(" and t.historica  = true ");

            if (semestre.HasValue)
                query.AppendLine($" and t.semestre = @semestre ");

            if (turmasCodigo != null && turmasCodigo.Any(t => t != "-99" && t != null))
                query.AppendLine($" and t.turma_id = any(@turmasCodigo) ");
            
            query.AppendLine(" union all  ");

            query.AppendLine(@" select distinct 
                            p.id || usu.rf_codigo as pendenciaId,
                            p.titulo,
	                        p.descricao as Descricao,
	                        p.situacao,
                            p.instrucao, 
	                        d.abreviacao as DreNome,
	                        te.descricao || ' - ' || u.nome as UeNome,
	                        t.ano_letivo as AnoLetivo,
	                        t.modalidade_codigo as ModalidadeCodigo,
	                        t.semestre,
                            t.nome ||
	                            case t.ano
		                            when '0' then ''
		                            else ' - '|| t.ano || 'ºANO'
	                            end as TurmaNome,
                            t.turma_id as TurmaCodigo,
	                        a.disciplina_id::bigint as DisciplinaId,
	                        pe.bimestre,
                            usu.nome as criador,
                            usu.login as criadorRf,
	                        p.alterado_por as aprovador,
	                        p.alterado_rf as aprovadorRf,
                            'Diário de Classe' as TipoPendencia,
                            false as OutrasPendencias,
                            p.tipo,
                            to_char(a.data_aula, 'dd/MM') ||' - '|| coalesce(pa.motivo, '') as Detalhe
                        from pendencia_aula pa
                        inner join pendencia p 
	                        on pa.pendencia_id  = p.id
	                    inner join aula a 
                            on a.id  = pa.aula_id
	               	    inner join turma t 
	                        on t.turma_id = a.turma_id
	                    inner join ue u 
	                        on t.ue_id  = u.id 
	                    inner join dre d 
	                        on u.dre_id  = d.id   
	                    inner join tipo_escola te
                            on te.cod_tipo_escola_eol = u.tipo_escola       
                        inner join pendencia_usuario pu 
                              on pu.pendencia_id = p.id
                        inner join usuario usu 
                              on usu.id = pu.usuario_id
	                    inner join tipo_calendario tc
		                    on a.tipo_calendario_id  = tc.id 
	                    inner join periodo_escolar pe
		                    on pe.tipo_calendario_id = tc.id 
                        where t.ano_letivo = @anoLetivo
                        and d.dre_id  = @dreCodigo
                        and u.ue_id  = @ueCodigo
                        and p.situacao in(1,2)
                        and not p.excluido ");

            if (modalidadeId > 0)
                query.AppendLine(" and t.modalidade_codigo = @modalidadeId");

            if (!String.IsNullOrEmpty(usuarioRf) && usuarioRf.Length > 0)
                query.AppendLine(" and usu.login = @usuarioRf ");

            if (semestre.HasValue)
                query.AppendLine($" and t.semestre = @semestre ");

            if (exibirHistorico)
                query.AppendLine(" and t.historica  = true ");

            if (turmasCodigo != null && turmasCodigo.Any(t => t != "-99" && t != null))
                query.AppendLine($" and t.turma_id = any(@turmasCodigo) ");

            if (componentesCodigo != null && componentesCodigo.Any(t => t != -99))
                query.AppendLine($" and a.disciplina_id::bigint = any(@componentesCodigo)");

            if (bimestre > 0)
                query.AppendLine($" and pe.bimestre  = @bimestre and a.data_aula between pe.periodo_inicio and pe.periodo_fim");
            
            
            query.AppendLine(" union all  ");

            query.AppendLine(@"select 
	                            distinct p.id || usu.rf_codigo as pendenciaId,
	                            p.titulo,
	                            p.descricao as Descricao,
	                            p.situacao,
                                p.instrucao,
                                d.abreviacao as DreNome,
                                te.descricao || ' - ' || u.nome as UeNome,
                                t.ano_letivo as AnoLetivo,
                                t.modalidade_codigo as ModalidadeCodigo,
                                t.semestre,
                                t.nome ||
	                                case t.ano
	                                    when '0' then ''
	                                    else ' - '|| t.ano || 'ºANO'
	                                end as TurmaNome,
                                t.turma_id as TurmaCodigo,
                                0 as DisciplinaId,
                                0 as bimestre,
                                usu.nome as criador,
                                usu.login as criadorRf,
                                p.alterado_por as aprovador,
                                p.alterado_rf as aprovadorRf,
                                'Diário de Classe' as TipoPendencia,
                                true as OutrasPendencias,
                                p.tipo,
                                '' as Detalhe
                            from pendencia_devolutiva pd 
	                            inner join pendencia p on pd.pendencia_id = p.id
	                            inner join turma t on t.id = pd.turma_id 
	                            inner join aula a on a.turma_id  = t.turma_id	
	                            inner join ue u on t.ue_id  = u.id 
	                            inner join dre d on u.dre_id  = d.id 
	                            inner join tipo_escola te on te.cod_tipo_escola_eol = u.tipo_escola       
                                inner join pendencia_usuario pu on pu.pendencia_id = p.id
                                inner join usuario usu on usu.id = pu.usuario_id
	                            inner join tipo_calendario tc on a.tipo_calendario_id  = tc.id 
                                inner join periodo_escolar pe on pe.tipo_calendario_id = tc.id 
                            where t.ano_letivo = @anoLetivo
                            and d.dre_id  = @dreCodigo
                            and u.ue_id  = @ueCodigo
                            and p.situacao in(1,2)
                            and not p.excluido  ");
            if (modalidadeId > 0)
                query.AppendLine(" and t.modalidade_codigo = @modalidadeId");
            if (!string.IsNullOrEmpty(usuarioRf) && usuarioRf.Length > 0)
                query.AppendLine(" and usu.login = @usuarioRf ");
            if (semestre.HasValue)
                query.AppendLine($" and t.semestre = @semestre ");
            if (exibirHistorico)
                query.AppendLine(" and t.historica  = true ");
            if (semestre.HasValue)
                query.AppendLine($" and t.semestre = @semestre ");
            if (turmasCodigo != null && turmasCodigo.Any(t => t != "-99" && t != null))
                query.AppendLine($" and t.turma_id = any(@turmasCodigo) ");
            if (componentesCodigo != null && componentesCodigo.Any(t => t != -99))
                query.AppendLine($" and a.disciplina_id::bigint = any(@componentesCodigo) ");

            query.AppendLine(" union all  ");
            query.AppendLine(@"select 
	                    distinct p.id || usu.rf_codigo as pendenciaId,
	                    p.titulo,
	                    p.descricao as Descricao,
	                    p.situacao,
                        p.instrucao,
                        d.abreviacao as DreNome,
                        te.descricao || ' - ' || u.nome as UeNome,
                        t.ano_letivo as AnoLetivo,
                        t.modalidade_codigo as ModalidadeCodigo,
                        t.semestre,
                        t.nome ||
	                        case t.ano
	                            when '0' then ''
	                            else ' - '|| t.ano || 'ºANO'
	                        end as TurmaNome,
                        t.turma_id as TurmaCodigo,
                        0 as DisciplinaId,
                        0 as bimestre,
                        usu.nome as criador,
                        usu.login as criadorRf,
                        p.alterado_por as aprovador,
                        p.alterado_rf as aprovadorRf,
                        'Diário de Classe' as TipoPendencia,
                        true as OutrasPendencias,
                        p.tipo,
                        '' as Detalhe
                    from pendencia_diario_bordo pdb 
	                    inner join pendencia p on pdb.pendencia_id = p.id
	                    inner join aula a on a.id = pdb.aula_id
	                    inner join turma t on t.turma_id = a.turma_id
	                    inner join ue u on t.ue_id  = u.id 
	                    inner join dre d on u.dre_id  = d.id 
	                    inner join tipo_escola te on te.cod_tipo_escola_eol = u.tipo_escola       
                        inner join pendencia_usuario pu on pu.pendencia_id = p.id
                        inner join usuario usu on usu.id = pu.usuario_id
	                    inner join tipo_calendario tc on a.tipo_calendario_id  = tc.id 
                        inner join periodo_escolar pe on pe.tipo_calendario_id = tc.id 
                            where t.ano_letivo = @anoLetivo
                            and d.dre_id  = @dreCodigo
                            and u.ue_id  = @ueCodigo
                            and p.situacao in(1,2)
                            and not p.excluido  ");
            if (modalidadeId > 0)
                query.AppendLine(" and t.modalidade_codigo = @modalidadeId");
            if (!string.IsNullOrEmpty(usuarioRf) && usuarioRf.Length > 0)
                query.AppendLine(" and usu.login = @usuarioRf ");
            if (semestre.HasValue)
                query.AppendLine($" and t.semestre = @semestre ");
            if (exibirHistorico)
                query.AppendLine(" and t.historica  = true ");
            if (semestre.HasValue)
                query.AppendLine($" and t.semestre = @semestre ");
            if (turmasCodigo != null && turmasCodigo.Any(t => t != "-99" && t != null))
                query.AppendLine($" and t.turma_id = any(@turmasCodigo) ");
            if (componentesCodigo != null && componentesCodigo.Any(t => t != -99))
                query.AppendLine($" and a.disciplina_id::bigint = any(@componentesCodigo) ");
            return query.ToString();
        }
    }
}
