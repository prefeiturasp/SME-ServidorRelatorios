using Dapper;
using SME.SR.Infra;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ObterEnderecoeAtosDaUeRepository : IObterEnderecoeAtosDaUeRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ObterEnderecoeAtosDaUeRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<CabecalhoDto> ObterEnderecoEAtos(string ueCodigo)
        {

            var query = @"SELECT DISTINCT
	                        RTRIM(LTRIM(vcue.nm_unidade_educacao)) NomeEscola,
	                        vcue.nm_logradouro as endereco,
	                        hu.cd_ato,
	                        hu.nr_ato,
	                        hu.dc_ato
                        FROM
                        historico_unidade hu 
                        INNER JOIN v_cadastro_unidade_educacao vcue  ON
	                        vcue.cd_unidade_educacao = hu.cd_unidade_educacao
                        WHERE
	                        hu.cd_unidade_educacao = @codigoUe";

            using (var conn = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                conn.Open();
                var cabecalho = await conn.QueryFirstOrDefaultAsync<CabecalhoDto>(query, new { ueCodigo });
                conn.Close();
                return cabecalho;
            }
        }
    }
}
