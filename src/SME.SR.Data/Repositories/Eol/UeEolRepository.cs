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
                    		    vcue.nm_bairro as Bairro,
                                (
                                  select top 1
                                     CONCAT(RTRIM(LTRIM(dcu.cd_ddd)), ' ', RTRIM(LTRIM(dcu.dc_dispositivo))) as telefone  
                                  from
                                     v_cadastro_unidade_educacao cue 
                                     inner join
                                        dispositivo_comunicacao_unidade dcu 
                                        on cue.cd_unidade_educacao = dcu.cd_unidade_educacao 
                                     inner join
                                        tipo_dispositivo_comunicacao tdc 
                                        on dcu.tp_dispositivo_comunicacao = tdc.tp_dispositivo_comunicacao 
                                  where
                                     cue.cd_unidade_educacao =  @ueCodigo 
                                     and tdc.tp_dispositivo_comunicacao = 1 
                               )
                               Telefone 
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
