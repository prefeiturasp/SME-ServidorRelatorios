using Dapper;
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
            var query = @"select id as ComunicadoId, titulo as Comunicado, data_envio as DataEnvio, data_expiracao as DataExpiracao
                          from comunicado
                         where ano_letivo = @AnoLetivo
                           and modalidade = @ModalidadeTurma";

            if (!string.IsNullOrEmpty(filtro.CodigoDre))
                query += " and codigo_dre = @CodigoDre ";
            if (!string.IsNullOrEmpty(filtro.CodigoUe))
                query += " and codigo_ue = @CodigoUe ";
            if (filtro.Semestre > 0)
                query += " and semestre = @Semestre ";
            if (filtro.DataInicio > DateTime.MinValue)
                query += " and data_envio between @DataInicio and @DataFim ";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryAsync<LeituraComunicadoDto>(query.ToString(), new
            {
                filtro.AnoLetivo,
                filtro.ModalidadeTurma,
                filtro.CodigoDre,
                filtro.CodigoUe,
                filtro.Semestre,
                filtro.DataInicio,
                filtro.DataFim
            });
        }

        public async Task<long[]> ObterComunicadoTurmasAlunosPorComunicadoId(long comunicado)
        {
            var query = @"select 
        	    distinct aluno_codigo::bigint
        from comunicado_aluno ca 
        where ca.comunicado_id = @comunicado";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
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

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryAsync<LeituraComunicadoTurmaDto>(query.ToString(), new { comunicados = comunicados.ToArray() });
        }

        public async Task<IEnumerable<LeituraComunicadoResponsavelDto>> ObterResponsaveisPorAlunosIds(long[] estudantes)
        {
            var query = @"SELECT [cd_identificador_responsavel] as ResponsavelId
                      ,LTRIM(RTRIM(cd_aluno)) as AlunoId
                      ,LTRIM(RTRIM([nm_responsavel])) as ResponsavelNome
                      ,'Filiação 1' as TipoResponsavel
                      ,LTRIM(RTRIM([cd_cpf_responsavel])) as CPF
                      ,case when nr_celular_responsavel is not null then '(' +LTRIM(RTRIM(cd_ddd_celular_responsavel)) + ') ' + LTRIM(RTRIM(nr_celular_responsavel)) else '' end as Contato
                  FROM [se1426].[dbo].[responsavel_aluno]
                  where cd_aluno in @estudantes";

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            return await conexao.QueryAsync<LeituraComunicadoResponsavelDto>(query, new { estudantes } );
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
	                select unl.notificacao_id, unl.dre_codigoeol, unl.ue_codigoeol, unnest(string_to_array(n.grupo,',')) modalidade, nt.codigo_eol_turma, count(distinct usuario_cpf) leram
	                from usuario_notificacao_leitura unl 
	                left join notificacao n on n.id = unl.notificacao_id
	                left join notificacao_turma nt on nt.notificacao_id = unl.notificacao_id 
	                group by unl.notificacao_id, unl.dre_codigoeol, unl.ue_codigoeol, unnest(string_to_array(n.grupo,',')), nt.codigo_eol_turma 
                ) ul on ul.notificacao_id = cn.notificacao_id and ul.dre_codigoeol::varchar = cn.dre_codigo and ul.ue_codigoeol = cn.ue_codigo and ul.modalidade = cn.modalidade_codigo::text and ul.codigo_eol_turma = cn.turma_codigo 
                where
	                cn.notificacao_id = ANY(@comunicados) and
	                cn.turma_codigo <> 0;";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringAE);
            return await conexao.QueryAsync<LeituraComunicadoTurmaDto>(query.ToString(), new { comunicados = comunicados.ToArray() });
        }

        public async Task<IEnumerable<LeituraComunicadoEstudanteDto>> ObterComunicadoTurmasEstudanteAppPorComunicadosIds(long[] comunicados)
        {
            var query = @"select 
        	usuario_id as UsuarioId,
        	usuario_cpf as UsuarioCpf, 
        	codigo_eol_aluno::varchar as CodigoEstudante, 
        	notificacao_id as ComunicadoId,
        	mensagemvisualizada as Situacao 
        from usuario_notificacao_leitura unl
        inner join notificacao n on unl.notificacao_id = n.id  
        where unl.notificacao_id = ANY(@comunicados);";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringAE);
            return await conexao.QueryAsync<LeituraComunicadoEstudanteDto>(query.ToString(), new { comunicados = comunicados });
        }

    }
}
