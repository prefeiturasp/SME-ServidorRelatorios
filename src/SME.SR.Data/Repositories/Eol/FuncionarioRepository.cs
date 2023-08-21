using Dapper;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class FuncionarioRepository : IFuncionarioRepository
    {

        private readonly VariaveisAmbiente variaveisAmbiente;

        public FuncionarioRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<Funcionario>> ObterFuncionariosPorCargoUe(string codigoCargo, string codigoUe)
        {
            var campos = @"
            SELECT 
	            NomeServidor
	            ,CodigoRF
				,Documento
	            ,DataInicio
	            ,DataFim
	            ,Cargo
	            FROM(
	                SELECT DISTINCT 
							servidor.nm_pessoa              NomeServidor 
			                ,servidor.cd_registro_funcional            CodigoRF 
							,CONCAT(servDetalhes.nr_rg_pessoa, '-', ISNULL(servDetalhes.cd_complemento_rg,'X'), '/', orgaoEmissor.nm_orgao_emissor) Documento
			                ,cargoServidor.dt_posse           DataInicio 
			                ,cargoServidor.dt_fim_nomeacao    DataFim  
	                        ,CASE WHEN cargoSobreposto.dc_cargo IS NOT NULL THEN cargoSobreposto.dc_cargo ELSE cargo.dc_cargo END AS Cargo
				            ,CASE WHEN cargoSobreposto.cd_cargo IS NOT NULL THEN cargoSobreposto.cd_cargo ELSE cargo.cd_cargo END AS cd_cargo
	                FROM v_servidor_cotic servidor
					INNER JOIN v_servidor_mstech_ativos AS servDetalhes ON servDetalhes.cd_registro_funcional = servidor.cd_registro_funcional
					INNER JOIN orgao_emissor AS orgaoEmissor ON orgaoEmissor.cd_orgao_emissor = servDetalhes.codigo_orgao_emissor_rg
	                INNER JOIN v_cargo_base_cotic AS cargoServidor ON cargoServidor.CD_SERVIDOR = servidor.cd_servidor
	                INNER JOIN cargo AS cargo ON cargoServidor.cd_cargo = cargo.cd_cargo
	                LEFT JOIN lotacao_servidor AS lotacao_servidor 
		                ON cargoServidor.cd_cargo_base_servidor = lotacao_servidor.cd_cargo_base_servidor
	                INNER JOIN v_cadastro_unidade_educacao dre 
		                ON lotacao_servidor.cd_unidade_educacao = dre.cd_unidade_educacao 
		            LEFT JOIN (
			            SELECT 
				             cargoSobreposto.cd_cargo
				            ,cargoSobreposto.dc_cargo
				            ,cargo_sobreposto_servidor.cd_cargo_base_servidor 
				            ,cargo_sobreposto_servidor.cd_unidade_local_servico
			            FROM cargo_sobreposto_servidor AS cargo_sobreposto_servidor 
				            INNER JOIN cargo AS cargoSobreposto ON cargo_sobreposto_servidor.cd_cargo = cargoSobreposto.cd_cargo
				            INNER JOIN lotacao_servidor AS lotacao_servidor_sobreposto 
					            ON cargo_sobreposto_servidor.cd_cargo_base_servidor = lotacao_servidor_sobreposto.cd_cargo_base_servidor
			            WHERE (cargo_sobreposto_servidor.dt_fim_cargo_sobreposto IS NULL
			            OR cargo_sobreposto_servidor.dt_fim_cargo_sobreposto > GETDATE())) cargoSobreposto
				            ON cargoSobreposto.cd_cargo_base_servidor = cargoServidor.cd_cargo_base_servidor
					            AND cargoSobreposto.cd_unidade_local_servico = dre.cd_unidade_educacao
	                WHERE  lotacao_servidor.dt_fim IS NULL AND dre.cd_unidade_educacao = @CodigoUE
							AND NOT EXISTS (SELECT 1 FROM cargo_sobreposto_servidor cargoSobreposto WHERE cargoSobreposto.cd_cargo_base_servidor = cargoServidor.cd_cargo_base_servidor AND dt_fim_nomeacao IS NULL)
		            UNION
		            SELECT DISTINCT servidor.nm_pessoa              NomeServidor 
			                ,servidor.cd_registro_funcional            CodigoRF 
							,CONCAT(servDetalhes.nr_rg_pessoa, '-', ISNULL(servDetalhes.cd_complemento_rg,'X'), '/', orgaoEmissor.nm_orgao_emissor) Documento
			                ,cargoServidor.dt_posse           DataInicio 
			                ,cargoServidor.dt_fim_nomeacao    DataFim  
			                ,RTRIM(LTRIM(cargo.dc_cargo))     Cargo
			                ,cargo.cd_cargo
	                FROM v_servidor_cotic servidor
						INNER JOIN v_servidor_mstech_ativos AS servDetalhes ON servDetalhes.cd_registro_funcional = servidor.cd_registro_funcional
						INNER JOIN orgao_emissor AS orgaoEmissor ON orgaoEmissor.cd_orgao_emissor = servDetalhes.codigo_orgao_emissor_rg
		                INNER JOIN v_cargo_base_cotic AS cargoServidor ON cargoServidor.CD_SERVIDOR = servidor.cd_servidor
		                LEFT JOIN lotacao_servidor AS lotacao_servidor ON cargoServidor.cd_cargo_base_servidor = lotacao_servidor.cd_cargo_base_servidor
		                INNER JOIN cargo_sobreposto_servidor AS cargo_sobreposto_servidor 
			                ON cargo_sobreposto_servidor.cd_cargo_base_servidor = cargoServidor.cd_cargo_base_servidor
			                AND (cargo_sobreposto_servidor.dt_fim_cargo_sobreposto IS NULL
			                OR cargo_sobreposto_servidor.dt_fim_cargo_sobreposto > GETDATE())
                        INNER JOIN cargo AS cargo ON cargo_sobreposto_servidor.cd_cargo = cargo.cd_cargo
		                INNER JOIN v_cadastro_unidade_educacao dre 
			                ON cargo_sobreposto_servidor.cd_unidade_local_servico = dre.cd_unidade_educacao
		            WHERE  lotacao_servidor.dt_fim IS NULL AND dre.cd_unidade_educacao = @CodigoUE
	                UNION
	                SELECT DISTINCT servidor.nm_pessoa              NomeServidor 
			                ,servidor.cd_registro_funcional            CodigoRF 
							,CONCAT(servDetalhes.nr_rg_pessoa, '-', ISNULL(servDetalhes.cd_complemento_rg,'X'), '/', orgaoEmissor.nm_orgao_emissor) Documento
			                ,cargoServidor.dt_posse           DataInicio 
			                ,cargoServidor.dt_fim_nomeacao    DataFim  
			                ,RTRIM(LTRIM(cargo.dc_cargo))     Cargo
			                ,cargo.cd_cargo
	                FROM v_servidor_cotic servidor
						INNER JOIN v_servidor_mstech_ativos AS servDetalhes ON servDetalhes.cd_registro_funcional = servidor.cd_registro_funcional
						INNER JOIN orgao_emissor AS orgaoEmissor ON orgaoEmissor.cd_orgao_emissor = servDetalhes.codigo_orgao_emissor_rg
		                INNER JOIN v_cargo_base_cotic AS cargoServidor ON cargoServidor.CD_SERVIDOR = servidor.cd_servidor
		                INNER JOIN cargo AS cargo ON cargoServidor.cd_cargo = cargo.cd_cargo
		                INNER JOIN atribuicao_aula atribuicao ON atribuicao.cd_cargo_base_servidor = cargoServidor.cd_cargo_base_servidor
		                INNER JOIN v_cadastro_unidade_educacao dre 
			                ON atribuicao.cd_unidade_educacao = dre.cd_unidade_educacao
	                WHERE   atribuicao.dt_cancelamento IS NULL 
		                AND dre.cd_unidade_educacao = @CodigoUE 
		                AND cargoServidor.dt_fim_nomeacao IS NULL
		                AND atribuicao.dt_disponibilizacao_aulas IS  NULL
		                AND YEAR(dt_atribuicao_aula)  = YEAR(GETDATE())
	                UNION
		                SELECT DISTINCT servidor.nm_pessoa              NomeServidor 
			                ,servidor.cd_registro_funcional            CodigoRF 
							,CONCAT(servDetalhes.nr_rg_pessoa, '-', ISNULL(servDetalhes.cd_complemento_rg,'X'), '/', orgaoEmissor.nm_orgao_emissor) Documento
			                ,cargoServidor.dt_posse           DataInicio 
			                ,cargoServidor.dt_fim_nomeacao    DataFim  
			                ,RTRIM(LTRIM(cargo.dc_cargo))     Cargo
			                ,cargo.cd_cargo
	                FROM v_servidor_cotic servidor
						INNER JOIN v_servidor_mstech_ativos AS servDetalhes ON servDetalhes.cd_registro_funcional = servidor.cd_registro_funcional
						INNER JOIN orgao_emissor AS orgaoEmissor ON orgaoEmissor.cd_orgao_emissor = servDetalhes.codigo_orgao_emissor_rg
		                INNER JOIN v_cargo_base_cotic AS cargoServidor ON cargoServidor.CD_SERVIDOR = servidor.cd_servidor
		                INNER JOIN cargo AS cargo ON cargoServidor.cd_cargo = cargo.cd_cargo
		                INNER JOIN funcao_atividade_cargo_servidor atividade ON atividade.cd_cargo_base_servidor = cargoServidor.cd_cargo_base_servidor
		                INNER JOIN v_cadastro_unidade_educacao dre 
			                ON atividade.cd_unidade_local_servico = dre.cd_unidade_educacao
	                WHERE   atividade.dt_fim_funcao_atividade IS NULL 
		                AND dre.cd_unidade_educacao = @CodigoUE 
            ) Funcionarios";

            var where = new StringBuilder();
            object parametros;
            if (!string.IsNullOrEmpty(codigoCargo))
            {
                where.Append(" WHERE funcionarios.cd_cargo = @CodigoCargo");
                parametros = new { CodigoUE = codigoUe, CodigoCargo = codigoCargo };
            }
            else
                parametros = new { CodigoUE = codigoUe };

            var query = MontaQueryObterFuncionariosPorCargoUe(campos, where.ToString(), " ORDER BY NomeServidor");

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryAsync<Funcionario>(query, parametros);
            }
        }

        public async Task<IEnumerable<Guid>> ObterPerfisUsuarioPorRf(string usuarioRf)
        {
            var query = @"SELECT UsGrup.Gru_Id
                          FROM SYS_UsuarioGrupo UsGrup 
                         INNER JOIN SYS_Grupo Grupo ON Grupo.gru_id = UsGrup.gru_id
                         INNER JOIN sys_usuario u on u.usu_id = UsGrup.usu_id
                         WHERE Grupo.sis_id = 1000
                           and u.usu_login = @usuarioRf
                           and usg_situacao = 1";

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringCoreSso))
            {
                return await conexao.QueryAsync<Guid>(query, new { usuarioRf });
            }
        }

        public async Task<IEnumerable<Funcionario>> ObterNomesServidoresPorRfs(string[] codigosRfs)
        {
            var query = @"select 
							cd_registro_funcional as CodigoRF,
							nm_pessoa as NomeServidor
						  from v_servidor_cotic
						  where cd_registro_funcional IN @codigosRfs ";

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryAsync<Funcionario>(query, new { codigosRfs });
            }
        }

        protected string MontaQueryObterFuncionariosPorCargoUe(string query, string where, string order = "")
        {
            var sb = new StringBuilder();
            sb.AppendLine(query);

            sb.AppendLine(where);

            if (!string.IsNullOrEmpty(order))
            {
                sb.AppendLine(order);
            }

            return sb.ToString();
        }
    }
}