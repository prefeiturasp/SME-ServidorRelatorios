using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Repositories.Sgp
{
    public class RelatorioSondagemPortuguesPorTurmaRepository : IRelatorioSondagemPortuguesPorTurmaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;
        public RelatorioSondagemPortuguesPorTurmaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }
        public async Task<IEnumerable<RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto>> ObterPlanilhaLinhas(string dreCodigo, long turmaCodigo, int ano, int semestre, ProficienciaSondagemEnum proficiencia)
        {
            string sql = String.Empty;

            switch (proficiencia)
            {
                case ProficienciaSondagemEnum.Leitura:
                    sql = $"";
                    break;
                case ProficienciaSondagemEnum.Escrita:
                    sql = $"";
                    break;
                case ProficienciaSondagemEnum.LeituraVozAlta:
                    sql = $"";
                    break;
            }

            if (sql == String.Empty)
                throw new Exception($"{ proficiencia } fora do esperado.");

            var parametros = new { dreCodigo, ano, turmaCodigo, semestre };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryAsync<RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto>(sql, parametros);
        }
    }
}
