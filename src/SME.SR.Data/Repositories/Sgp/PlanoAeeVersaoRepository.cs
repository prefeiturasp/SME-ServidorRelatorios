using System;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;

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
    }
}