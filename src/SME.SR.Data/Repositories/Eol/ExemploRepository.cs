using Dapper;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ExemploRepository : IExemploRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ExemploRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }
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

            using (var conn = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                conn.Open();
                var ue = await conn.QueryFirstOrDefaultAsync<UeDto>(query, new { codigoUe });
                conn.Close();
                return ue;
            }
        }
    }
}
