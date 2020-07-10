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
            try
            {
                var query = @"select cac.id, cac.nome, cc.idcomponentecurricular CodigoComponenteCurricular from componentecurricular cc
                          left join componentecurricularareadoconhecimento cac on cac.id = cc.idareadoconhecimento
                          where cc.idcomponentecurricular = ANY(@CodigosComponentesCurriculares)  ";

                var parametros = new { CodigosComponentesCurriculares = codigosComponentesCurriculares };

                using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringApiEol))
                {
                    return await conexao.QueryAsync<AreaDoConhecimento>(query, parametros);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
