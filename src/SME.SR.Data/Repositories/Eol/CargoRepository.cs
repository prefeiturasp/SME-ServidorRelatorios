using Dapper;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class CargoRepository : ICargoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public CargoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<ServidorCargoDto>> BuscaCargosRfPorAnoLetivo(string[] codigosRF, int anoLetivo)
        {
            var campos = @"Select distinct RTRIM(LTRIM(servidor.nm_pessoa)) NomeServidor,
				                    servidor.cd_registro_funcional CodigoRF,
                                    RTRIM(LTRIM(cargo.dc_cargo)) NomeCargo,
				                    cargo.cd_cargo CodigoCargo,
                                    RTRIM(LTRIM(cargo_2.dc_cargo)) NomeCargoSobreposto,
				                    cargo_2.cd_cargo CodigoCargoSobreposto
                        from v_servidor_cotic servidor
                        inner
                        join v_cargo_base_cotic as cargo_base_servidor
                      on cargo_base_servidor.CD_SERVIDOR = servidor.cd_servidor

                      INNER JOIN cargo as cargo

                                on cargo_base_servidor.cd_cargo = cargo.cd_cargo
                            LEFT JOIN lotacao_servidor as lotacao_servidor 
                                on cargo_base_servidor.cd_cargo_base_servidor = lotacao_servidor.cd_cargo_base_servidor
                            LEFT JOIN  cargo_sobreposto_servidor as cargo_sobreposto_servidor

                                on cargo_sobreposto_servidor.cd_cargo_base_servidor = cargo_base_servidor.cd_cargo_base_servidor

                                    AND(cargo_sobreposto_servidor.dt_fim_cargo_sobreposto IS NULL

                                    OR year(cargo_sobreposto_servidor.dt_fim_cargo_sobreposto) > @anoLetivo)
                            LEFT JOIN cargo as cargo_2 on cargo_sobreposto_servidor.cd_cargo = cargo_2.cd_cargo
                             {0}    --WHERE
                             UNION ALL
                            select distinct RTRIM(LTRIM(servidor.nm_pessoa)) NomeServidor,
				                        servidor.cd_registro_funcional CodigoRF,
                                        RTRIM(LTRIM(cargo.dc_cargo)) NomeCargo, 
				                        cargo.cd_cargo CodigoCargo, 
                                       '' NomeCargoSobreposto,
				                       '' CodigoCargoSobreposto
                            from v_servidor_cotic servidor
                           inner join v_cargo_base_cotic as cargo_base_servidor on cargo_base_servidor.CD_SERVIDOR = servidor.cd_servidor
                            LEFT JOIN lotacao_servidor as lotacao_servidor

                                on cargo_base_servidor.cd_cargo_base_servidor = lotacao_servidor.cd_cargo_base_servidor
                            LEFT JOIN  cargo_sobreposto_servidor as cargo_sobreposto_servidor

                                on cargo_sobreposto_servidor.cd_cargo_base_servidor = cargo_base_servidor.cd_cargo_base_servidor

                                    AND(cargo_sobreposto_servidor.dt_fim_cargo_sobreposto IS NULL

                                    OR year(cargo_sobreposto_servidor.dt_fim_cargo_sobreposto) > @anoLetivo)
                            LEFT JOIN cargo on cargo_sobreposto_servidor.cd_cargo = cargo.cd_cargo
                {0}  --WHERE";

            var where = "where cd_registro_funcional in (@codigosRF) and dt_fim_nomeacao is null";
            var query = string.Format(campos, where);
            var parametros = new { codigosRF, anoLetivo };

            using (var conn = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                var result = await conn.QueryAsync<ServidorCargoDto>(query, parametros);
                return result.ToList();
            }
        }
    }
}
