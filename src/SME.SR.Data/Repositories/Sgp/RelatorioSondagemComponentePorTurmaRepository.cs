using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data.Repositories.Sgp
{
    public class RelatorioSondagemComponentePorTurmaRepository : IRelatorioSondagemComponentePorTurmaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public RelatorioSondagemComponentePorTurmaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }
        public async Task<RelatorioSondagemComponentesPorTurmaRelatorioDto> ObterRelatorio(int dreId, int turmaId, int ueId, int ano)
        {
            // TODO: Implementar ObterRelatorio - Marcos Rezende
            var query = new StringBuilder();
            query.Append(@" aqui vai a query");

            var parametros = new { dreId, ueId, turmaId, ano };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                return await Task.FromResult(new RelatorioSondagemComponentesPorTurmaRelatorioDto());
            }
        }
    }
}
