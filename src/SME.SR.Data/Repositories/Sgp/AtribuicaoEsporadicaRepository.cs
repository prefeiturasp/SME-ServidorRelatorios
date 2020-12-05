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
            var sql = MontaQueryCompleta(codigoRF, dreId, ueId);

            var parametros = new { inicioAno = new DateTime(anoLetivo, 1, 1), fimAno = new DateTime(anoLetivo, 12, 31), dreId, ueId, codigoRF };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);

            return await conexao.QueryAsync<AtribuicaoEsporadica>(sql, parametros);
        }

        private string MontaQueryCompleta(string codigoRF, string dreId, string ueId)
        {
            StringBuilder sql = new StringBuilder();

            MontaQueryConsulta(sql, codigoRF, dreId, ueId);

            return sql.ToString();
        }

        private static void MontaQueryConsulta(StringBuilder sql, string codigoRF, string dreId, string ueId)
        {
            ObtenhaCabecalho(sql);

            ObtenhaFiltro(sql, codigoRF, dreId, ueId);

            sql.AppendLine("order by id desc");
        }

        private static void ObtenhaCabecalho(StringBuilder sql)
        {
            sql.AppendLine(@$"select professor_rf ProfessorRf, ue_id UeId, dre_id DreId, data_inicio DataInicio, data_fim DataFim,  
                                     criado_em CriadoEm, criado_por CriadoPor from atribuicao_esporadica where excluido = false");
        }

        private static void ObtenhaFiltro(StringBuilder sql, string codigoRF, string dreId, string ueId)
        {
            if (!string.IsNullOrWhiteSpace(codigoRF))
                sql.AppendLine("and professor_rf = @codigoRF");

            if (!string.IsNullOrWhiteSpace(dreId))
                sql.AppendLine("and dre_id = @dreId");

            if (!string.IsNullOrWhiteSpace(ueId))
                sql.AppendLine("and ue_id = @ueId");

            sql.AppendLine("and data_inicio >= @inicioAno and data_inicio <= @fimAno");
            sql.AppendLine("and data_fim >= @inicioAno and data_fim <= @fimAno");
        }
    }
}
