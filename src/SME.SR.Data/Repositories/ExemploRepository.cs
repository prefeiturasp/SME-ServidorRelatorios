using Dapper;
using SME.SR.Infra.Dtos;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ExemploRepository : IExemploRepository
    {

        public async Task<string> ObterGames()
        {
            return await Task.FromResult("Quake");
        }


        public async Task<UeDto> ObterUePorCodigo(string codigoUe)
        {

            var query = @"
                        SELECT
	                        esc.cd_escola CodigoEscola,
	                        RTRIM(LTRIM(vcue.nm_unidade_educacao)) NomeEscola
                        FROM
	                        escola esc
                        INNER JOIN v_cadastro_unidade_educacao vcue ON
	                        esc.cd_escola = vcue.cd_unidade_educacao
                        WHERE
	                        esc.cd_escola = @codigoUe";

            using (var conn = new SqlConnection(""))
            {
                conn.Open();
                var ue = await conn.QueryFirstOrDefaultAsync<UeDto>(query, new { codigoUe });
                conn.Close();
                return ue;
            }
        }
    }
}
