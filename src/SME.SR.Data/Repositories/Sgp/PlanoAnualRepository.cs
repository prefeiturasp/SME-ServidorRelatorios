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
    public class PlanoAnualRepository : IPlanoAnualRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public PlanoAnualRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }
        
        public async Task<PlanoAnualDto> ObterPorId(long Id)
        {
            var query = @"select pa.id,
   								 ue.nome as Ue,
          						 te.descricao as TipoEscola,
						         dre.abreviacao as Dre,
								 t.modalidade_codigo as ModalidadeTurma,
						         t.nome as Turma,
							     t.turma_id as TurmaCodigo,
							     coalesce(cc.descricao_sgp, cc.descricao_infantil) as Disciplina
					from planejamento_anual pa
						join turma t on t.id = pa.turma_id 
						join componente_curricular cc on cc.id = pa.componente_curricular_id 
						join ue on t.ue_id = ue.id
						join tipo_escola te on ue.tipo_escola = te.id
					    join dre on ue.dre_id = dre.id
						where pa.id = @Id
						  and pa.excluido = false";

            var parametros = new { Id};
            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryFirstOrDefaultAsync<PlanoAnualDto>(query, parametros);
        }
        
        public async Task<IEnumerable<BimestreDescricaoObjetivosPlanoAnualDto>> ObterObjetivosPorPlanoAulaId(long Id)
        {
	        var query = @"select pe.bimestre, 
							     pac.descricao DescricaoPlanejamento, 
							     oa.codigo CodigoObjetivo,
							     oa.descricao DescricaoObjetivo
					from planejamento_anual pa
						join planejamento_anual_periodo_escolar pape on pape.planejamento_anual_id = pa.id 
					    join planejamento_anual_componente pac on pac.planejamento_anual_periodo_escolar_id = pape.id 
					    join planejamento_anual_objetivos_aprendizagem paoa on paoa.planejamento_anual_componente_id = pac.id 
					    join periodo_escolar pe on pe.id = pape.periodo_escolar_id 
					    join objetivo_aprendizagem oa on oa.id = paoa.objetivo_aprendizagem_id 
						where pa.id = @Id
						  and pape.excluido = false
					      and pa.excluido = false
					      and pac.excluido = false
					      and (paoa.excluido is null or paoa.excluido = false)
			         order by pe.bimestre,oa.codigo ";

	        var parametros = new { Id};
	        using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
	        return await conexao.QueryAsync<BimestreDescricaoObjetivosPlanoAnualDto>(query, parametros);
        }
    }
}