using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class AreaDoConhecimentoRepository : IAreaDoConhecimentoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public AreaDoConhecimentoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<AreaDoConhecimento>> ObterAreasDoConhecimentoPorComponentesCurriculares(long[] codigosComponentesCurriculares)
        {
            var query = @"select cac.id, cac.nome, cc.descricao_sgp as NomeComponenteCurricular, cc.id CodigoComponenteCurricular, o.Ordem 
                          from componente_curricular cc
                            left join componente_curricular_area_conhecimento cac on cac.id = cc.area_conhecimento_id
                            inner join componente_curricular_grupo_area_ordenacao o on o.area_conhecimento_id = cac.id 
                          where cc.id = ANY(@CodigosComponentesCurriculares)
                          order by ordem ";

            var parametros = new { CodigosComponentesCurriculares = codigosComponentesCurriculares };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<AreaDoConhecimento>(query, parametros);
            }
        }
    }
}
