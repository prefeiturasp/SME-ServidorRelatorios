using Dapper;
using SME.SR.Infra;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class UeEolRepository : IUeEolRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public UeEolRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }
        public async Task<UeEolEnderecoDto> ObterEnderecoUePorCodigo(long ueCodigo)
        {
            var query = @"SELECT 	
                                vcue.cd_unidade_educacao as UeCodigo,
                    		    tl.dc_tp_logradouro as TipoLogradouro,
                    		    vcue.nm_logradouro as Logradouro,
                    		    vcue.dc_complemento_endereco as ComplementoEndereco,
                    		    vcue.cd_nr_endereco as Numero,
                    		    vcue.nm_bairro as Bairro
						   FROM v_cadastro_unidade_educacao vcue 
						  INNER JOIN escola esc 
                             ON esc.cd_escola = vcue.cd_unidade_educacao
						  INNER JOIN tipo_logradouro tl 
						     ON vcue.tp_logradouro = tl.tp_logradouro
						  WHERE vcue.cd_unidade_educacao = @ueCodigo;";

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);

            return await conexao.QueryFirstOrDefaultAsync<UeEolEnderecoDto>(query, new { @ueCodigo });

        }
    }
}
