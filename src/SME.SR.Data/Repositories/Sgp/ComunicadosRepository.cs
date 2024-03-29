﻿using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ComunicadosRepository : IComunicadosRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ComunicadosRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<LeituraComunicadoDto>> ObterComunicadosPorFiltro(FiltroRelatorioLeituraComunicadosDto filtro)
        {
            var query = @"select comunicado.id as ComunicadoId, comunicado.titulo as Comunicado, comunicado.data_envio as DataEnvio, comunicado.data_expiracao as DataExpiracao
                          from comunicado ";

            if (filtro.Grupos != null && filtro.Grupos.Any())
            {
                query += $@" INNER JOIN comunidado_grupo cg ON comunicado.id = cg.comunicado_id ";
            }

            if (!string.IsNullOrEmpty(filtro.Turma))
            {
                query += $@" INNER JOIN comunicado_turma ct ON comunicado.id = ct.comunicado_id ";
            }

            query += " where comunicado.ano_letivo = @AnoLetivo ";

            if (filtro.Grupos != null && filtro.Grupos.Any())
            {
                query += $@" AND cg.grupo_comunicado_id = ANY(@Grupos) ";
            }

            if (!string.IsNullOrEmpty(filtro.NotificacaoId))
                query += " and comunicado.id = @NotificacaoId ";

            if (!string.IsNullOrEmpty(filtro.CodigoDre) && filtro.CodigoDre != "-99")
                query += " and codigo_dre = @CodigoDre ";

            if (filtro.CodigoDre == "-99")
                query += " and codigo_dre is null ";

            if (!string.IsNullOrEmpty(filtro.CodigoUe) && filtro.CodigoUe != "-99")
                query += " and codigo_ue = @CodigoUe ";

            if (filtro.CodigoUe == "-99")
                query += " and codigo_ue is null ";

            if (filtro.Semestre > 0)
                query += " and semestre = @Semestre ";

            if (!filtro.ListarComunicadosExpirados)
                query += " and data_expiracao >= @DataExpiracao ";

            query += " and date(data_envio) between @DataInicio and @DataFim and not comunicado.excluido  and comunicado.tipo_comunicado <>9 ";

            if (!string.IsNullOrEmpty(filtro.Turma))
            {
                query += " and ct.turma_codigo = @Turma ";
            }

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryAsync<LeituraComunicadoDto>(query.ToString(), new
            {
                NotificacaoId = long.Parse(!string.IsNullOrEmpty(filtro.NotificacaoId) ? filtro.NotificacaoId : "0"),
                filtro.AnoLetivo,
                filtro.CodigoDre,
                filtro.CodigoUe,
                filtro.Semestre,
                filtro.Grupos,
                filtro.Turma,
                DataInicio = filtro.DataInicio.GetValueOrDefault().Date,
                DataFim = filtro.DataFim.GetValueOrDefault().Date,
                DataExpiracao = DateTime.Now.Date
            });

        }

        public async Task<long[]> ObterComunicadoTurmasAlunosPorComunicadoId(long comunicado)
        {
            var query = @"select 
        	    distinct aluno_codigo::bigint
        from comunicado_aluno ca 
        where ca.comunicado_id = @comunicado";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return (await conexao.QueryAsync<long>(query.ToString(), new { comunicado })).AsList().ToArray();
        }
        public async Task<IEnumerable<LeituraComunicadoTurmaDto>> ObterComunicadoTurmasPorComunicadosIds(IEnumerable<long> comunicados)
        {
            var query = @"select 
        	comunicado_id as ComunicadoId,
        	t.nome as TurmaNome,
            t.turma_id as TurmaCodigo,
        	t.modalidade_codigo as TurmaModalidade,
        	0 as NaoInstalado,
        	0 as NaoVisualizado,
        	0 as Visualizado
        from comunicado_turma ct 
        inner join turma t on ct.turma_codigo = t.turma_id
        where ct.comunicado_id = ANY(@comunicados)";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryAsync<LeituraComunicadoTurmaDto>(query.ToString(), new { comunicados = comunicados.ToArray() });
        }

        public async Task<IEnumerable<LeituraComunicadoResponsavelDto>> ObterResponsaveisPorAlunosIds(int[] estudantes)
        {
            var query = @"SELECT [cd_identificador_responsavel] as ResponsavelId
                      ,LTRIM(RTRIM(cd_aluno)) as AlunoId
                      ,LTRIM(RTRIM([nm_responsavel])) as ResponsavelNome
                      , tp_pessoa_responsavel as TipoResponsavel
                      ,LTRIM(RTRIM([cd_cpf_responsavel])) as CPF
                      ,case when nr_celular_responsavel is not null then '(' +LTRIM(RTRIM(cd_ddd_celular_responsavel)) + ') ' + LTRIM(RTRIM(nr_celular_responsavel)) else '' end as Contato
                  FROM [se1426].[dbo].[responsavel_aluno]
                  where dt_fim is null and cd_aluno in @estudantes";

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            return await conexao.QueryAsync<LeituraComunicadoResponsavelDto>(query, new { estudantes });
        }

        public async Task<IEnumerable<LeituraComunicadoTurmaDto>> ObterComunicadoTurmasAppPorComunicadosIds(IEnumerable<long> comunicados)
        {
            var query = @"select distinct
        			cn.notificacao_id as ComunicadoId,
	                cn.modalidade_codigo TurmaModalidade,
	                cn.turma_codigo::varchar TurmaCodigo,
	                cn.quantidade_responsaveis_sem_app NaoInstalado,
	                cn.quantidade_responsaveis_com_app - coalesce(leram, 0) NaoVisualizado,
	                coalesce(leram, 0) Visualizado 
                from consolidacao_notificacao cn
                left join (select distinct dre_codigo, dre_nome from dashboard_adesao) da on da.dre_codigo = cn.dre_codigo
                left join 
                (
	                select unl.notificacao_id, unl.dre_codigoeol, unl.ue_codigoeol, unnest(string_to_array(n.modalidades,',')) modalidade, nt.codigo_eol_turma, count(distinct usuario_cpf) leram
	                from usuario_notificacao_leitura unl 
	                left join notificacao n on n.id = unl.notificacao_id
	                left join notificacao_turma nt on nt.notificacao_id = unl.notificacao_id 
	                group by unl.notificacao_id, unl.dre_codigoeol, unl.ue_codigoeol, unnest(string_to_array(n.modalidades,',')), nt.codigo_eol_turma 
                ) ul on ul.notificacao_id = cn.notificacao_id and ul.dre_codigoeol::varchar = cn.dre_codigo and ul.ue_codigoeol = cn.ue_codigo and ul.modalidade = cn.modalidade_codigo::text and ul.codigo_eol_turma = cn.turma_codigo 
                where
	                cn.notificacao_id = ANY(@comunicados) and
	                cn.turma_codigo <> 0;";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringAE);
            return await conexao.QueryAsync<LeituraComunicadoTurmaDto>(query.ToString(), new { comunicados = comunicados.ToArray() });
        }

        public async Task<IEnumerable<LeituraComunicadoEstudanteDto>> ObterComunicadoTurmasEstudanteAppPorComunicadosIds(long[] comunicados)
        {
            //    var query = @"select 
            //	usuario_id as UsuarioId,
            //	usuario_cpf as UsuarioCpf, 
            //	codigo_eol_aluno::varchar as CodigoEstudante, 
            //	notificacao_id as ComunicadoId,
            //	mensagemvisualizada as Situacao 
            //from usuario_notificacao_leitura unl
            //inner join notificacao n on unl.notificacao_id = n.id  
            //where unl.notificacao_id = ANY(@comunicados);";

            var query = @"select distinct 
        	u.id as UsuarioId, 
        	u.cpf as UsuarioCpf,
        	codigo_eol_aluno::varchar as CodigoEstudante, 
        	notificacao_id as ComunicadoId,
        	case when ud.codigo_dispositivo is not null then 1 else 0 end as Instalado,
        	mensagemvisualizada as Situacao 
        from usuario u 
        inner join usuario_notificacao_leitura unl on u.id = unl.usuario_id 
        inner join notificacao n on unl.notificacao_id = n.id  
        left join usuario_dispositivo ud on u.id = ud.usuario_id 
        where not u.excluido and n.id = ANY(@comunicados);";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringAE);
            return await conexao.QueryAsync<LeituraComunicadoEstudanteDto>(query.ToString(), new { comunicados = comunicados });
        }

        public async Task<IEnumerable<LeituraComunicadoDto>> ObterComunicadoDadosSMEPorComunicadosIds(IEnumerable<long> comunicados)
        {
            var query = @"select distinct
        			cn.notificacao_id as ComunicadoId,
	                cn.quantidade_responsaveis_sem_app NaoInstalado,
	                cn.quantidade_responsaveis_com_app - coalesce(leram, 0) NaoVisualizado,
	                coalesce(leram, 0) Visualizado 
                from consolidacao_notificacao cn
                left join (select distinct dre_codigo, dre_nome from dashboard_adesao) da on da.dre_codigo = cn.dre_codigo
                left join 
                (
	                select unl.notificacao_id, unl.dre_codigoeol, unl.ue_codigoeol, unnest(string_to_array(n.modalidades,',')) modalidade, nt.codigo_eol_turma, count(distinct usuario_cpf) leram
	                from usuario_notificacao_leitura unl 
	                left join notificacao n on n.id = unl.notificacao_id
	                left join notificacao_turma nt on nt.notificacao_id = unl.notificacao_id 
	                group by unl.notificacao_id, unl.dre_codigoeol, unl.ue_codigoeol, unnest(string_to_array(n.modalidades,',')), nt.codigo_eol_turma 
                ) ul on ul.notificacao_id = cn.notificacao_id and ul.dre_codigoeol::varchar = cn.dre_codigo and ul.ue_codigoeol = cn.ue_codigo and ul.modalidade = cn.modalidade_codigo::text and ul.codigo_eol_turma = cn.turma_codigo 
                where
	                cn.notificacao_id = ANY(@comunicados) and
	                cn.dre_codigo = '' and cn.ue_codigo = '';";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringAE);
            return await conexao.QueryAsync<LeituraComunicadoDto>(query.ToString(), new { comunicados = comunicados.ToArray() });
        }

        public async Task<string[]> ObterUsuariosApp()
        {
            var query = @"select distinct cpf from usuario u where not primeiroacesso;";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringAE);
            return (await conexao.QueryAsync<string>(query.ToString())).ToArray();

        }
    }
}
