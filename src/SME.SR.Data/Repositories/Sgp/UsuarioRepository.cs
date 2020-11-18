using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
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
            string query = @"select rf_codigo as CodigoRf, login as Login, nome as Nome from usuario where rf_codigo = @codigoRf";

            var parametros = new { codigoRf };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<Usuario>(query, parametros);
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

        public async Task<IEnumerable<DadosUsuarioDto>> ObterUsuariosAbrangenciaPorAcesso(string dreCodigo, string ueCodigo, string usuarioRf, string[] perfis, int diasSemAcesso)
        {
            var query = new StringBuilder(@"select distinct a.dre_abreviacao as Dre
	                                        , ue.tipo_escola as TipoEscola
	                                        , ue.Nome as Ue
	                                        , a.usuario_perfil as CodigoPerfil
	                                        , pp.nome_perfil as Perfil
	                                        , pp.tipo as TipoPerfil
	                                        , u.rf_codigo as Rf
	                                        , u.Nome
	                                        , u.ultimo_login as UltimoAcesso
                                        from public.v_abrangencia a 
                                         inner join ue on ue.ue_id = a.ue_codigo
                                         inner join usuario u on u.id = usuario_id
                                         inner join prioridade_perfil pp on pp.codigo_perfil = a.usuario_perfil
                                          where true ");

            if (!string.IsNullOrEmpty(dreCodigo))
                query.AppendLine("and a.dre_codigo = @dreCodigo");

            if (!string.IsNullOrEmpty(ueCodigo))
                query.AppendLine("and a.ue_codigo = @ueCodigo");

            if (!string.IsNullOrEmpty(usuarioRf))
                query.AppendLine("and u.rf_codigo = @usuarioRf");

            if (perfis.Any())
                query.AppendLine("and a.usuario_perfil = Any(@perfis)");

            if (diasSemAcesso > 0)
                query.AppendLine("and u.ultimo_login <= NOW() - interval '@diasSemAcesso day'");

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<DadosUsuarioDto>(query.ToString(), new { dreCodigo, ueCodigo, usuarioRf, perfis, diasSemAcesso });
            }
        }
    }
}
