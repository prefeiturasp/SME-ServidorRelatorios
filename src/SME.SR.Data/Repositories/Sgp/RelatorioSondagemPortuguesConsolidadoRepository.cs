using Dapper;
using Npgsql;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Repositories.Sgp
{
    public class RelatorioSondagemPortuguesConsolidadoRepository : IRelatorioSondagemPortuguesConsolidadoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;
        public RelatorioSondagemPortuguesConsolidadoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }
        public async Task<IEnumerable<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaQueryDto>> ObterPlanilha(string dreCodigo, string ueCodigo, string turmaCodigo, int anoLetivo, int anoTurma, int bimestre)
        {
            var sql = String.Empty;

            sql += "select ";
            sql += "o.\"Descricao\" Ordem, ";
            sql += "p.\"Descricao\" Pergunta, ";
            sql += "r.\"Descricao\" Resposta, ";
            sql += "count(*) Quantidade ";
            sql += "from \"Sondagem\" sa ";
            sql += "inner join \"ComponenteCurricular\" cc on sa.\"ComponenteCurricularId\" = cc.\"Id\" ";
            sql += "inner join \"Pergunta\" p on p.\"ComponenteCurricularId\" = cc.\"Id\" ";
            sql += "inner join \"PerguntaResposta\" pr on p.\"Id\" = pr.\"PerguntaId\" ";
            sql += "inner join \"Resposta\" r on pr.\"RespostaId\" = r.\"Id\" ";
            sql += "inner join \"Grupo\" g2 on sa.\"GrupoId\" = g2.\"Id\" ";
            sql += "inner join \"Ordem\" o on sa.\"OrdemId\" = o.\"Id\" and o.\"GrupoId\" = g2.\"Id\" ";
            sql += "inner join \"Periodo\" p2 on sa.\"PeriodoId\" = p2.\"Id\" ";
            sql += "where cc.\"Id\" = @componenteCurricularId ";
            sql += "and sa.\"CodigoDre\" = @dreCodigo ";
            sql += "and sa.\"CodigoUe\" = @ueCodigo ";
            sql += "and sa.\"AnoLetivo\" = @anoLetivo ";
            sql += "and sa.\"AnoTurma\" = @anoTurma ";
            sql += "and p2.\"Descricao\" = @periodo ";
            sql += "and sa.\"GrupoId\" = @grupoId ";
            sql += "group by Ordem, Pergunta, Resposta ";
            sql += "order by Ordem, Pergunta, Resposta ";

            var componenteCurricularId = ComponenteCurricularSondagemEnum.Portugues.Name();
            var grupoId = GrupoSondagemEnum.CapacidadeLeitura.Name();

            var periodo = $"{ bimestre }° Bimestre";

            var parametros = new { grupoId, componenteCurricularId, periodo, dreCodigo, ueCodigo, turmaCodigo, anoLetivo, anoTurma };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryAsync<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaQueryDto>(sql, parametros);
        }
    }
}
