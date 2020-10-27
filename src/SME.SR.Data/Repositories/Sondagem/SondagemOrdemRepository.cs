using Dapper;
using Npgsql;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class SondagemOrdemRepository : ISondagemOrdemRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public SondagemOrdemRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }
        public async Task<IEnumerable<SondagemOrdemDto>> ObterPorGrupo(GrupoSondagemEnum grupoSondagem)
        {
            String sql = String.Empty;

            sql = $"select distinct o.\"Id\", o.\"Descricao\", o.\"Ordenacao\" ";
            sql += "from \"Ordem\" o ";
            sql += "where o.\"GrupoId\" = @grupoId ";
            sql += "order by o.\"Ordenacao\" ";

            var parametros = new { grupoId = grupoSondagem.Name() };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryAsync<SondagemOrdemDto>(sql, parametros);
        }

    }
}
