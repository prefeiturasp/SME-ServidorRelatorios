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

            var where = "where cd_registro_funcional in @codigosRF and dt_fim_nomeacao is null";
            var query = string.Format(campos, where);
            var parametros = new { codigosRF, anoLetivo };

            using (var conn = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                var result = await conn.QueryAsync<ServidorCargoDto>(query, parametros);
                return result.ToList();
            }
        }

        public async Task<IEnumerable<ServidorCargoDto>> BuscarCargosAtividades(string[] codigosRF)
        {
            var query = @"
            SELECT 
                    cd_registro_funcional CodigoRf
                    ,NomeServidor
                    ,cd_cargo AS CodigoCargo
		            ,cd_tipo_funcao AS CodigoTipoFuncao
                    ,Sobreposto
                    ,CodigoComponenteCurricular
		            ,CodigoCargoBase
            FROM(
	            SELECT  
                    servidor.cd_registro_funcional    
                    ,RTRIM(LTRIM(servidor.nm_pessoa)) NomeServidor
		            ,cargo.cd_cargo
		            ,null AS cd_tipo_funcao
                    ,0 as Sobreposto
                    ,NULL as CodigoComponenteCurricular
		            ,cargoServidor.cd_cargo_base_servidor as CodigoCargoBase
	            FROM v_servidor_cotic servidor
	            INNER JOIN v_cargo_base_cotic AS cargoServidor ON cargoServidor.CD_SERVIDOR = servidor.cd_servidor
	            INNER JOIN cargo AS cargo ON cargoServidor.cd_cargo = cargo.cd_cargo
	            INNER JOIN lotacao_servidor AS lotacao_servidor 
		            ON cargoServidor.cd_cargo_base_servidor = lotacao_servidor.cd_cargo_base_servidor
	            INNER JOIN v_cadastro_unidade_educacao ue ON lotacao_servidor.cd_unidade_educacao = ue.cd_unidade_educacao
	            LEFT JOIN escola ON ue.cd_unidade_educacao = escola.cd_escola
	            WHERE  lotacao_servidor.dt_fim IS NULL AND servidor.cd_registro_funcional IN @codigosRF
		            AND ((escola.tp_escola IS NOT NULL AND escola.tp_escola IN (1,3,4,16,2,17,20,28,31)) OR escola.tp_escola IS NULL) --EMEF,EMEFM,EMEBS, CEU EMEF
	            UNION
	            SELECT  
                     servidor.cd_registro_funcional
                    ,RTRIM(LTRIM(servidor.nm_pessoa)) NomeServidor
                    ,cargo.cd_cargo
		            ,null AS cd_tipo_funcao
                    ,1 as Sobreposto
                    ,NULL as CodigoComponenteCurricular
		            ,cargoServidor.cd_cargo_base_servidor as CodigoCargoBase
	            FROM v_servidor_cotic servidor
		            INNER JOIN v_cargo_base_cotic AS cargoServidor ON cargoServidor.CD_SERVIDOR = servidor.cd_servidor
		            LEFT JOIN lotacao_servidor AS lotacao_servidor ON cargoServidor.cd_cargo_base_servidor = lotacao_servidor.cd_cargo_base_servidor
		            INNER JOIN cargo_sobreposto_servidor AS cargo_sobreposto_servidor 
			            ON cargo_sobreposto_servidor.cd_cargo_base_servidor = cargoServidor.cd_cargo_base_servidor
			            AND (cargo_sobreposto_servidor.dt_fim_cargo_sobreposto IS NULL
			            OR cargo_sobreposto_servidor.dt_fim_cargo_sobreposto > GETDATE())
                    INNER JOIN cargo AS cargo ON cargo_sobreposto_servidor.cd_cargo = cargo.cd_cargo
		            INNER JOIN v_cadastro_unidade_educacao ue ON cargo_sobreposto_servidor.cd_unidade_local_servico = ue.cd_unidade_educacao
		            LEFT JOIN escola ON ue.cd_unidade_educacao = escola.cd_escola
	            WHERE  lotacao_servidor.dt_fim IS NULL AND servidor.cd_registro_funcional IN @codigosRF
		            AND ((escola.tp_escola IS NOT NULL AND escola.tp_escola IN (1,3,4,16,2,17,20,28,31)) OR escola.tp_escola IS NULL) --EMEF,EMEFM,EMEBS, CEU EMEF
	            UNION
	            SELECT  
                    servidor.cd_registro_funcional
                    ,RTRIM(LTRIM(servidor.nm_pessoa)) NomeServidor
                    ,cargo.cd_cargo
		            ,null AS cd_tipo_funcao
                    ,0 as Sobreposto
                    ,componente.cd_componente_curricular as CodigoComponenteCurricular
		            ,cargoServidor.cd_cargo_base_servidor as CodigoCargoBase
	            FROM v_servidor_cotic servidor
		            INNER JOIN v_cargo_base_cotic AS cargoServidor ON cargoServidor.CD_SERVIDOR = servidor.cd_servidor
		            INNER JOIN cargo AS cargo ON cargoServidor.cd_cargo = cargo.cd_cargo
		            INNER JOIN atribuicao_aula atribuicao ON atribuicao.cd_cargo_base_servidor = cargoServidor.cd_cargo_base_servidor
                    LEFT JOIN componente_curricular componente 
	                    ON atribuicao.cd_componente_curricular = componente.cd_componente_curricular
		                AND componente.dt_cancelamento IS NULL 
		            INNER JOIN v_cadastro_unidade_educacao ue ON atribuicao.cd_unidade_educacao = ue.cd_unidade_educacao
		            LEFT JOIN escola ON ue.cd_unidade_educacao = escola.cd_escola
	            WHERE   atribuicao.dt_cancelamento IS NULL 
		            AND servidor.cd_registro_funcional IN @codigosRF
		            AND cargoServidor.dt_fim_nomeacao IS NULL
		            AND atribuicao.dt_disponibilizacao_aulas IS  NULL
		            AND an_atribuicao  = YEAR(GETDATE())
		            AND ((escola.tp_escola IS NOT NULL AND escola.tp_escola IN (1,3,4,16,2,17,20,28,31)) OR escola.tp_escola IS NULL) --EMEF,EMEFM,EMEBS, CEU EMEF
	            UNION
	            SELECT  
                    servidor.cd_registro_funcional
                    ,RTRIM(LTRIM(servidor.nm_pessoa)) NomeServidor
                    ,cargo.cd_cargo
		            ,atividade.cd_tipo_funcao
                    ,0 as Sobreposto
                    ,NULL as CodigoComponenteCurricular
		            ,cargoServidor.cd_cargo_base_servidor as CodigoCargoBase
	            FROM v_servidor_cotic servidor
		            INNER JOIN v_cargo_base_cotic AS cargoServidor ON cargoServidor.CD_SERVIDOR = servidor.cd_servidor
		            INNER JOIN cargo AS cargo ON cargoServidor.cd_cargo = cargo.cd_cargo
		            INNER JOIN funcao_atividade_cargo_servidor atividade ON atividade.cd_cargo_base_servidor = cargoServidor.cd_cargo_base_servidor
		            INNER JOIN v_cadastro_unidade_educacao ue ON atividade.cd_unidade_local_servico = ue.cd_unidade_educacao
		            LEFT JOIN escola ON ue.cd_unidade_educacao = escola.cd_escola
	            WHERE   atividade.dt_fim_funcao_atividade IS NULL 
		            AND servidor.cd_registro_funcional IN @codigosRF
		            AND ((escola.tp_escola IS NOT NULL AND escola.tp_escola IN (1,3,4,16,2,17,20,28,31)) OR escola.tp_escola IS NULL) --EMEF,EMEFM,EMEBS, CEU EMEF
	            ) Cargos
                GROUP BY  
                    cd_registro_funcional 
                    ,NomeServidor
                    ,cd_cargo 
		            ,cd_tipo_funcao
					,CodigoComponenteCurricular
                    ,Sobreposto
		            ,CodigoCargoBase";

            var parametros = new { codigosRF };

            using (var conn = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                var result = await conn.QueryAsync<ServidorCargoDto>(query, parametros);
                return result.ToList();
            }
        }
    }
}
