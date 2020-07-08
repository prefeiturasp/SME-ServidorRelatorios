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

        public async Task<IEnumerable<AreaDoConhecimento>> ObterAreasDoConhecimentoPorComponentesCurriculares(string[] codigosComponentesCurriculares)
        {
            try
            {


                var query = @"select cac.id, cac.nome, cc.idcomponentecurricular from componentecurricularareadoconhecimento cac 
                          inner join componentecurricular cc on cac.id = cc.idareadoconhecimento
                          where cc.idcomponentecurricular = ANY(@CodigosComponentesCurriculares)  ";

                var parametros = new { CodigosComponentesCurriculares = codigosComponentesCurriculares };

                using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringApiEol))
                {
                    var listaAreas = new Dictionary<long, AreaDoConhecimento>();

                    return (await conexao.QueryAsync<AreaDoConhecimento, long, AreaDoConhecimento>(query,
                               (areaDoConhecimento, idcomponentecurricular) =>
                               {
                                   AreaDoConhecimento areaDoConhecimentoObj;

                                   if (!listaAreas.TryGetValue(areaDoConhecimento.Id, out areaDoConhecimentoObj))
                                   {
                                       areaDoConhecimentoObj = areaDoConhecimento;
                                       areaDoConhecimentoObj.ComponentesCurricularesId = new List<long>();
                                       listaAreas.Add(areaDoConhecimentoObj.Id, areaDoConhecimentoObj);
                                   }

                                   areaDoConhecimentoObj.ComponentesCurricularesId =
                                        areaDoConhecimentoObj.ComponentesCurricularesId.Append(idcomponentecurricular);

                                   return areaDoConhecimentoObj;
                               },
                               parametros, splitOn: "id, idcomponentecurricular"))
                           .Distinct()
                           .ToList();
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
