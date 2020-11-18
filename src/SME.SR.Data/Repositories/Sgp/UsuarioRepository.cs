using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<HistoricoReinicioSenhaDto>> ObterHistoricoReinicioSenhaUsuarioPorDre(string codigoDre)
        {
            string query = @"select hrs.ue_codigo, hrs.usuario_rf, hrs.criado_em as Senha_Reiniciada, hrs.criado_por as senha_reiniciada_por
                              from historico_reinicio_senha hrs
                             where dre_codigo = @codigoDre
                               and hrs.id in (select hrs1.id 
				                                from historico_reinicio_senha hrs1
				                               where hrs1.usuario_rf = hrs.usuario_rf
				                               order by criado_em desc 
				                               limit 10)
                                order by hrs.ue_codigo, hrs.usuario_rf, hrs.criado_em desc";            

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<HistoricoReinicioSenhaDto>(query, new { codigoDre });
            }
        }
    }
}
