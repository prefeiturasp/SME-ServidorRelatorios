using Dapper;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<EnderecoEAtosDaUeDto>> ObterEnderecoEAtos(string ueCodigo)
        {
            var query = @"SELECT DISTINCT
	                        CONCAT(RTRIM(LTRIM(tpe.sg_tp_escola)), ' - ', RTRIM(LTRIM(vcue.nm_unidade_educacao))) AS nomeUe,
	                        CONCAT(tl.dc_tp_logradouro, ' ', vcue.nm_logradouro, ', ', vcue.cd_nr_endereco, ', ', vcue.nm_bairro, ', ', vcue.cd_cep, ' SÃO PAULO - SP') as endereco,
	                        CONCAT(hu.nr_ato, ' - ', convert(varchar, hu.dt_publicacao_dom, 103)) as atos,
	                        tp_ocorrencia_historica as tipo_ocorrencia
                        FROM
                        historico_unidade hu 
                        INNER JOIN v_cadastro_unidade_educacao vcue ON
	                        hu.cd_unidade_educacao = vcue.cd_unidade_educacao 
                        INNER JOIN tipo_logradouro tl ON
	                        vcue.tp_logradouro = tl.tp_logradouro
                        INNER JOIN escola esc ON
	                        esc.cd_escola = vcue.cd_unidade_educacao 
                        INNER JOIN tipo_escola tpe ON
	                        tpe.tp_escola = esc.tp_escola
                        WHERE
	                        tp_ocorrencia_historica in (1, 7)
	                        amd hu.cd_unidade_educacao = @codigoUe";

            using (var conn = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                conn.Open();
                var enderecosEAtos = await conn.QueryAsync<IEnumerable<EnderecoEAtosDaUeDto>>(query, new { ueCodigo });
                conn.Close();
                return (IEnumerable<EnderecoEAtosDaUeDto>) enderecosEAtos;
            }
        }
    }
}
