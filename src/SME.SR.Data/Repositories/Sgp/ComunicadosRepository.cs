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

        public async Task<IEnumerable<LeituraComunicadoResponsaveoDto>> ObterResponsaveisPorAlunosIds(long[] estudantes)
        {
            var query = @"SELECT [cd_identificador_responsavel] as ResponsavelId
                      ,[nm_responsavel] as Nome
                      ,'Filiação 1' as TipoResponsavel
                      ,[cd_cpf_responsavel] as CPF
                      ,case when nr_celular_responsavel is not null then '(' + cd_ddd_celular_responsavel + ') ' + nr_celular_responsavel else '' end as Contato
                  FROM [se1426].[dbo].[responsavel_aluno]
                  where cd_aluno in @estudantes";

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            return await conexao.QueryAsync<LeituraComunicadoResponsaveoDto>(query, new { estudantes } );
        }
    }
}
