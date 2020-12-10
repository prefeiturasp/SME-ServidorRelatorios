using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class UsuarioAERepository : IUsuarioAERepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public UsuarioAERepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }
        public async Task<IEnumerable<UsuarioAEDto>> ObterUsuarioAEPorCpfs(string[] cpfs)
        {
            var query = new StringBuilder(@" select distinct cpf, ultimologin, primeiroacesso, excluido 
                           from usuario where cpf = any(@cpfs) ");
         

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringAE))
            {
                return await conexao.QueryAsync<UsuarioAEDto>(query.ToString(), new { cpfs });
            }
        }
    }
}
