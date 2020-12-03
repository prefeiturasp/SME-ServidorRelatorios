using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class AtribuicaoEsporadicaRepository : IAtribuicaoEsporadicaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public AtribuicaoEsporadicaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<AtribuicaoEsporadica>> ObterPorFiltros(int anoLetivo, string dreId, string ueId, string codigoRF)
        {
            var sql = MontaQueryCompleta(codigoRF);

            var parametros = new { inicioAno = new DateTime(anoLetivo, 1, 1), fimAno = new DateTime(anoLetivo, 12, 31), dreId, ueId, codigoRF };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);

            return await conexao.QueryAsync<AtribuicaoEsporadica>(sql, parametros);
        }

        private string MontaQueryCompleta(string codigoRF)
        {
            StringBuilder sql = new StringBuilder();

            MontaQueryConsulta(sql, codigoRF, contador: false);

            sql.AppendLine(";");

            MontaQueryConsulta(sql, codigoRF, contador: true);

            return sql.ToString();
        }

        private static void MontaQueryConsulta(StringBuilder sql, string codigoRF, bool contador = false)
        {
            ObtenhaCabecalho(sql, contador);

            ObtenhaFiltro(sql, codigoRF);

            if (!contador)
                sql.AppendLine("order by id desc");
        }

        private static void ObtenhaCabecalho(StringBuilder sql, bool contador)
        {
            sql.AppendLine($"select {(contador ? "count(*)" : "id,professor_rf,ue_id,dre_id,data_inicio, data_fim")} from atribuicao_esporadica where excluido = false");
        }

        private static void ObtenhaFiltro(StringBuilder sql, string codigoRF)
        {
            if (!string.IsNullOrWhiteSpace(codigoRF))
                sql.AppendLine("and professor_rf = @codigoRF");

            sql.AppendLine("and dre_id = @dreId");
            sql.AppendLine("and ue_id = @ueId");
            sql.AppendLine("and data_inicio >= @inicioAno and data_inicio <= @fimAno");
            sql.AppendLine("and data_fim >= @inicioAno and data_fim <= @fimAno");
        }
    }
}
