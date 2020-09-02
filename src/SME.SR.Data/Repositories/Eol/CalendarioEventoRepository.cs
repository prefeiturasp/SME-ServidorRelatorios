using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class CalendarioEventoRepository : ICalendarioEventoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public CalendarioEventoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }
        public async Task<IEnumerable<CalendarioEventoQueryRetorno>> ObterEventosPorUsuarioTipoCalendarioPerfilDreUe(string usuarioLogin, Guid usuarioPerfil, bool consideraHistorico, bool consideraPendenteAprovacao,
            string dreCodigo, string ueCodigo, bool desconsideraEventoSme, bool desconsideraLocalDre, long tipoCalendarioId)
        {
            var query = @"select distinct e.id,
					e.data_inicio as dataInicio,
					case
						when data_inicio = data_fim then ''
						else '(inicio)'
					end InicioFimDesc,
					e.nome,
					case
						when e.dre_id is not null and e.ue_id is null then 'DRE'
				      	when e.dre_id is not null and e.ue_id is not null then 'UE'
						else 'SME'
					end tipoEvento
		from evento e
			inner join evento_tipo et
				on e.tipo_evento_id = et.id
			inner join tipo_calendario tc
				on e.tipo_calendario_id = tc.id
			left join f_abrangencia_dres(@usuarioLogin, @usuarioPerfil, @consideraHistorico) ad
				on e.dre_id = ad.codigo 
				-- modalidade 1 (fundamental/medio) do tipo de calendario, considera as modalidades 5 (Fundamental) e 6 (medio)
				-- modalidade 2 (EJA) do tipo de calendario, considera modalidade 3 (EJA)
				and ((tc.modalidade = 1 and ad.modalidade_codigo in (5, 6)) or (tc.modalidade = 2 and ad.modalidade_codigo = 3))
				-- para DREs considera local da ocorrencia 2 (DRE) e 5 (Todos)
				and et.local_ocorrencia in (2, 5)
			left join f_abrangencia_ues(@usuarioLogin, @usuarioPerfil, @consideraHistorico) au
				on e.ue_id = au.codigo
				and ((tc.modalidade = 1 and au.modalidade_codigo in (5, 6)) or (tc.modalidade = 2 and au.modalidade_codigo = 3))
				-- para UEs considera local da ocorrencia 1 (UE) e 4 (SME/UE) e 5 (Todos)
				and et.local_ocorrencia in (1, 4, 5)
	where et.ativo 
		and not et.excluido
		and not e.excluido		
		and extract(year from e.data_inicio) = tc.ano_letivo	
		and e.tipo_calendario_id = @tipoCalendarioId
		-- caso considere 1 (aprovado) e 2 (pendente de aprovacao), senao considera so aprovados
		and ((@consideraPendenteAprovacao = true and e.status in (1,2)) or (@consideraPendenteAprovacao = false and e.status = 1)) 
		and ((@dreCodigo is null and ((e.dre_id is null and e.ue_id is null) or e.dre_id in (select codigo from f_abrangencia_dres(@usuarioLogin, @usuarioPerfil, @consideraHistorico)))) or (@dreCodigo is not null and ((e.dre_id is null and e.ue_id is null) or e.dre_id = @dreCodigo)))
		and ((@ueCodigo is null and (e.ue_id is null or e.ue_id in (select codigo from f_abrangencia_ues(@usuarioLogin, @usuarioPerfil, @consideraHistorico)))) or (@ueCodigo is not null and (e.ue_id is null or e.ue_id = @ueCodigo)))
		-- caso desconsidere o local do evento 2 (DRE)
		and (@desconsideraLocalDre = false or (@desconsideraLocalDre = true and et.local_ocorrencia != 2))
		-- caso desconsidere evento SME
		and (@desconsideraEventoSme = false or (@desconsideraEventoSme = true and not (e.dre_id is null and e.ue_id is null)))
		
	union distinct
	
	select distinct e.id,
					e.data_fim,
					'(fim)' as InicioFimDesc,
					e.nome,
					case
						when e.dre_id is not null and e.ue_id is null then 'DRE'
				      	when e.dre_id is not null and e.ue_id is not null then 'UE'
						else 'SME'
					end tipoEvento
		from evento e
			inner join evento_tipo et
				on e.tipo_evento_id = et.id
			inner join tipo_calendario tc
				on e.tipo_calendario_id = tc.id
			left join f_abrangencia_dres(@usuarioLogin, @usuarioPerfil, @consideraHistorico) ad
				on e.dre_id = ad.codigo 
				and ((tc.modalidade = 1 and ad.modalidade_codigo in (5, 6)) or (tc.modalidade = 2 and ad.modalidade_codigo = 3))
				and et.local_ocorrencia in (2, 5)
			left join f_abrangencia_ues(@usuarioLogin, @usuarioPerfil, @consideraHistorico) au
				on e.ue_id = au.codigo
				and ((tc.modalidade = 1 and au.modalidade_codigo in (5, 6)) or (tc.modalidade = 2 and au.modalidade_codigo = 3))
				and et.local_ocorrencia in (1, 4, 5)
	where e.data_inicio <> e.data_fim
		and et.ativo 
		and not et.excluido
		and not e.excluido		
		and extract(year from e.data_inicio) = tc.ano_letivo
		and e.tipo_calendario_id = @tipoCalendarioId
		-- caso considere 1 (aprovado) e 2 (pendente de aprovacao), senao considera so aprovados
		and ((@consideraPendenteAprovacao = true and e.status in (1,2)) or (@consideraPendenteAprovacao = false and e.status = 1)) 
		and ((@dreCodigo is null and ((e.dre_id is null and e.ue_id is null) or e.dre_id in (select codigo from f_abrangencia_dres(@usuarioLogin, @usuarioPerfil, @consideraHistorico)))) or (@dreCodigo is not null and ((e.dre_id is null and e.ue_id is null) or e.dre_id = @dreCodigo)))
		and ((@ueCodigo is null and (e.ue_id is null or e.ue_id in (select codigo from f_abrangencia_ues(@usuarioLogin, @usuarioPerfil, @consideraHistorico)))) or (@ueCodigo is not null and (e.ue_id is null or e.ue_id = @ueCodigo)))
		-- caso desconsidere o local do evento 2 (DRE)
		and (@desconsideraLocalDre = false  or (@desconsideraLocalDre = true and et.local_ocorrencia != 2))
		-- caso desconsidere evento SME
		and (@desconsideraEventoSme = false or (@desconsideraEventoSme = true and not (e.dre_id is null and e.ue_id is null)));	";


            dreCodigo = string.IsNullOrEmpty(dreCodigo) ? null : dreCodigo;
            ueCodigo = string.IsNullOrEmpty(ueCodigo) ? null : ueCodigo;

            var parametros = new { usuarioLogin, usuarioPerfil, consideraHistorico, consideraPendenteAprovacao,  dreCodigo, ueCodigo, desconsideraEventoSme, desconsideraLocalDre = !desconsideraLocalDre, tipoCalendarioId };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);

            return await conexao.QueryAsync<CalendarioEventoQueryRetorno>(query, parametros);

        }
    }
}
