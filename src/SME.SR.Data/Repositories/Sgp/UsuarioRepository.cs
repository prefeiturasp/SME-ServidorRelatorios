using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public UsuarioRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<Usuario> ObterDados(string codigoRf)
        {
            string query = @"select rf_codigo as CodigoRf, login as Login, nome as Nome from usuario where rf_codigo = @codigoRf or login = @codigoRf";

            var parametros = new { codigoRf };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<Usuario>(query, parametros);
            }
        }

        public async Task<UsuarioCoreSSO> ObterDadosCoreSSO(string codigoRf)
        {
            string query = @"select 
                                  pes_nome as Nome
                                , usu_senha AS Senha
                                , usu_criptografia AS TipoCriptografia
                                , usu_situacao as Situacao 
                                from SYS_Usuario 
                               inner join PES_Pessoa on SYS_Usuario.pes_id = PES_Pessoa.pes_id
                               where usu_login = @codigoRf";

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringCoreSso))
            {
                return await conexao.QueryFirstOrDefaultAsync<UsuarioCoreSSO>(query, new { codigoRf });
            }
        }

        public async Task<SituacaoUsuario> ObterSituacaoUsuarioPorRf(string usuarioRf)
        {
            string query = @"select usu_situacao from SYS_Usuario where usu_login = @usuarioRf";

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringCoreSso))
            {
                return await conexao.QueryFirstOrDefaultAsync<SituacaoUsuario>(query, new { usuarioRf });
            }
        }

        public async Task<string> ObterNomeUsuarioPorLogin(string usuarioLogin)
        {
            string query = @"select pes_nome as nome from PES_Pessoa pp where pes_nome_abreviado = @usuarioLogin";

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringCoreSso))
            {
                return await conexao.QueryFirstOrDefaultAsync<string>(query, new { usuarioLogin });
            }
        }

        public async Task<IEnumerable<DadosUsuarioDto>> ObterUsuariosAbrangenciaPorAcesso(string dreCodigo, string ueCodigo, string usuarioRf, Guid[] perfis, int diasSemAcesso)
        {
            var query = new StringBuilder(@"select distinct a.dre_codigo as DreCodigo
	                                        , a.dre_abreviacao as Dre
                                            , ue.tipo_escola as TipoEscola
	                                        , ue.Nome as Ue
	                                        , a.usuario_perfil as PerfilGuid
	                                        , pp.nome_perfil as Perfil
	                                        , pp.tipo as TipoPerfil
	                                        , u.login
	                                        , u.Nome
	                                        , u.ultimo_login as UltimoAcesso
                                        from public.v_abrangencia a 
                                         inner join ue on ue.ue_id = a.ue_codigo
                                         inner join usuario u on u.id = usuario_id
                                         inner join prioridade_perfil pp on pp.codigo_perfil = a.usuario_perfil
                                          where true ");

            if (!string.IsNullOrEmpty(dreCodigo) && !dreCodigo.Equals("-99"))
                query.AppendLine("and a.dre_codigo = @dreCodigo");

            if (!string.IsNullOrEmpty(ueCodigo) && !ueCodigo.Equals("-99"))
                query.AppendLine("and a.ue_codigo = @ueCodigo");

            if (!string.IsNullOrEmpty(usuarioRf))
                query.AppendLine("and u.rf_codigo = @usuarioRf");

            if (perfis.Any())
                query.AppendLine("and a.usuario_perfil = Any(@perfis)");

            if (diasSemAcesso > 0)
                query.AppendLine($"and u.ultimo_login <= NOW() - interval '{diasSemAcesso} day'");

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<DadosUsuarioDto>(query.ToString(), new { dreCodigo, ueCodigo, usuarioRf, perfis });
            }
        }

        public async Task<IEnumerable<HistoricoReinicioSenhaDto>> ObterHistoricoReinicioSenhaUsuarioPorDre(string codigoDre)
        {
            string query = @"select u.login, u.Nome , to_char(hrs.criado_em, 'DD/MM/YYYY HH24:MI') as SenhaReiniciada, hrs.criado_por as SenhaReiniciadaPor, hrs.criado_rf as SenhaReiniciadaPorRf
                              from historico_reinicio_senha hrs
                             inner join usuario u on u.rf_codigo = hrs.usuario_rf
                             where dre_codigo = @codigoDre
                               and hrs.id in (select hrs1.id 
				                                from historico_reinicio_senha hrs1
				                               where hrs1.usuario_rf = hrs.usuario_rf
				                               order by criado_em desc 
				                               limit 10)
                                order by hrs.ue_codigo, hrs.usuario_rf, hrs.criado_em desc";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<HistoricoReinicioSenhaDto>(query, new { codigoDre });
            }
        }

        public async Task<IEnumerable<PrioridadePerfil>> ObterListaPrioridadePerfil()
        {
            string query = @"select codigo_perfil as CodigoPerfil, nome_perfil as NomePerfil, ordem, tipo
                            from public.prioridade_perfil";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<PrioridadePerfil>(query);
            }
        }

        public async Task<IEnumerable<Usuario>> ObterNomesUsuariosPorRfs(string[] codigosRfs)
        {
            string query = @"select usu_login as codigoRf, pes_nome as Nome 
                             from SYS_Usuario 
                             inner join PES_Pessoa on SYS_Usuario.pes_id = PES_Pessoa.pes_id
                             where usu_login IN @codigosRfs";

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringCoreSso))
            {
                return await conexao.QueryAsync<Usuario>(query, new { codigosRfs });
            }
        }
    }
}
