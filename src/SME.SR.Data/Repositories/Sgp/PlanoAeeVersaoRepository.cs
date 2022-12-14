using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Extensions;

namespace SME.SR.Data
{
    public class PlanoAeeVersaoRepository : IPlanoAeeVersaoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public PlanoAeeVersaoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<PlanoAeeDto> ObterPlanoAeePorVersaoPlanoId(long versaoPlanoId)
        {
	        const string query = @"select pav.numero as versaoplano,
										cast(coalesce(pav.alterado_em, pav.criado_em) as date) as dataversaoplano,
										pa.situacao as situacaoplano,
										pa.aluno_codigo as alunocodigo,
										pa.aluno_nome as alunonome,
										pa.parecer_coordenacao as parecercoordenacao,
										pa.parecer_paai as parecerpaai,
										pa.responsavel_paai_id as responsavelpaaiid,
										u2.nome as responsavelpaainome,
										coalesce(u2.login, u2.rf_codigo) as responsavelpaailoginrf,
										pa.responsavel_id as responsavelid,
										u3.nome as responsavelnome,
										coalesce(u3.login, u3.rf_codigo) as responsavelloginrf,
										t.nome as turmanome,
										t.ano_letivo as anoletivo,
										t.modalidade_codigo as modalidade,
										u.ue_id as uecodigo,
										u.nome as uenome,
										u.tipo_escola as tipoescola,
										d.nome as drenome,
										d.abreviacao as dreabreviacao
									from plano_aee_versao pav 
										inner join plano_aee pa on pa.id = pav.plano_aee_id 
										inner join turma t on t.id = pa.turma_id
										inner join ue u on u.id = t.ue_id 
										inner join dre d on d.id = u.dre_id
										left join usuario u2 on u2.id = pa.responsavel_paai_id
										left join usuario u3 on u3.id = pa.responsavel_id
									where pav.id = @versaoPlanoId";
	        
	        await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

	        return await conexao.QuerySingleOrDefaultAsync<PlanoAeeDto>(query, new { versaoPlanoId }); 
        }

        public async Task<IEnumerable<PlanosAeeDto>> ObterPlanoAEE(FiltroRelatorioPlanosAeeDto filtro)
        {
	        string query = @"with planosAtuais as
							(
								select distinct pa.id plano_aee_id, Max(pav.id) over (partition by pa.id) plano_aee_versao_id 
								from plano_aee pa 
									join plano_aee_versao pav on pa.id = pav.plano_aee_id
							)
							select 	pa.id, d.dre_id dreId, d.nome as dreNome,
								d.abreviacao as dreAbreviacao,
								u.ue_id as ueCodigo,
								u.nome as ueNome,
								u.tipo_escola as tipoEscola,
								pa.aluno_codigo as alunoCodigo,
								pa.aluno_nome as alunoNome,
								t.turma_id as turmaCodigo,
								t.nome as turmaNome,
								t.ano_letivo as anoLetivo,
								t.modalidade_codigo as modalidade,
								pa.situacao as situacaoPlano,
								u3.nome as responsavelNome,
								coalesce(u3.login, u3.rf_codigo) as responsavelLoginRf,
								pav.numero as versaoPlano,
								cast(coalesce(pav.alterado_em, pav.criado_em) as date) as dataVersaoPlano,
								u2.nome as responsavelPaaiNome,
								coalesce(u2.login, u2.rf_codigo) as responsavelPaaiLoginRf
							from plano_aee pa  
								join planosAtuais pAt on pAt.plano_aee_id = pa.id
								join plano_aee_versao pav on pa.id  = pav.plano_aee_id and pav.id = pAt.plano_aee_versao_id
								inner join turma t on t.id = pa.turma_id
								inner join ue u on u.id = t.ue_id 
								inner join dre d on d.id = u.dre_id
								left join usuario u2 on u2.id = pa.responsavel_paai_id
								left join usuario u3 on u3.id = pa.responsavel_id
							where 1 = 1	";

	        if (!filtro.DreCodigo.EstaFiltrandoTodas())
		        query += " and d.dre_id = @dreCodigo ";
	        
	        if (!filtro.Modalidade.EstaFiltrandoTodas())
		        query += " and t.modalidade_codigo = @modalidade ";
	        
	        if (!filtro.UeCodigo.EstaFiltrandoTodas())
		        query += " and u.ue_id = @ueCodigo ";
										  
	        if (filtro.ExibirEncerrados)
		        query += " and pa.situacao = 7 ";
	        else if (filtro.SituacaoIds != null && filtro.SituacaoIds.Any())
		        query += " and pa.situacao = ANY(@situacaoIds) ";
	        
	        if (!filtro.CodigosTurma.EstaFiltrandoTodas())
		        query += " and t.turma_id = ANY(@codigosTurma) ";
	        
	        if (filtro.CodigosResponsavel != null && filtro.CodigosResponsavel.Any())
		        query += " and coalesce(u3.login, u3.rf_codigo) = ANY(@codigosResponsavel) ";
	        
	        if (filtro.CodigosPAAIResponsavel != null && filtro.CodigosPAAIResponsavel.Any())
		        query += " and coalesce(u2.login, u2.rf_codigo) = ANY(@codigosPAAIResponsavel) ";	
									      
	        if (filtro.Semestre > 0)
		        query += " and t.semestre = @semestre ";
	        
	        query += " order by d.dre_id ";
	        
	        await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

	        var retorno = await conexao.QueryAsync<PlanosAeeDto>(query, new
	        {
		        dreCodigo = filtro.DreCodigo, modalidade = filtro.Modalidade, ueCodigo = filtro.UeCodigo,
		        situacaoIds = filtro.SituacaoIds, codigosResponsavel = filtro.CodigosResponsavel,
		        codigosPAAIResponsavel = filtro.CodigosPAAIResponsavel,
		        semestre = filtro.Semestre, codigosTurma = filtro.CodigosTurma
	        });

	        return retorno;
        }
    }
}