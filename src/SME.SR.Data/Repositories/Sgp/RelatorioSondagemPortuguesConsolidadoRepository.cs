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
        public async Task<IEnumerable<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaQueryDto>> ObterPlanilha(string dreCodigo, string ueCodigo, string turmaCodigo, int anoLetivo, int anoTurma, int bimestre, GrupoSondagemEnum grupo)
        {
            var sql = String.Empty;

            sql += "select ";
            sql += "o.\"Descricao\" Ordem, ";
            sql += "p.\"Descricao\" Pergunta, ";
            sql += "r.\"Descricao\" Resposta, ";
            sql += "count(*) Quantidade ";

            sql += "from \"Ordem\" o ";
            sql += "inner join \"GrupoOrdem\" go2 on go2.\"OrdemId\" = o.\"Id\"  ";
            sql += "inner join \"OrdemPergunta\" op on op.\"GrupoId\" = go2.\"GrupoId\"  ";
            sql += "inner join \"Pergunta\" p on p.\"Id\" = op.\"PerguntaId\"  ";
            sql += "inner join \"PerguntaResposta\" pr on pr.\"PerguntaId\" = p.\"Id\"  ";
            sql += "inner join \"Resposta\" r on r.\"Id\" = pr.\"RespostaId\"  ";
            sql += "inner join \"ComponenteCurricular\" cc on p.\"ComponenteCurricularId\" = cc.\"Id\"  ";
            sql += "inner join \"Sondagem\" sa on sa.\"ComponenteCurricularId\" = cc.\"Id\" and sa.\"GrupoId\" = go2.\"GrupoId\" and sa.\"OrdemId\" = o.\"Id\"  ";
            sql += "inner join \"Periodo\" p2 on sa.\"PeriodoId\" = p2.\"Id\" ";

            sql += "where cc.\"Id\" = @componenteCurricularId ";

            if (dreCodigo != null && dreCodigo != "0")
                sql += "and sa.\"CodigoDre\" = @dreCodigo ";

            if (ueCodigo != null && ueCodigo != String.Empty)
                sql += "and sa.\"CodigoUe\" = @ueCodigo ";

            sql += "and sa.\"AnoLetivo\" = @anoLetivo ";
            sql += "and sa.\"AnoTurma\" = @anoTurma ";
            sql += "and p2.\"Descricao\" = @periodo ";
            sql += "and sa.\"GrupoId\" = @grupoId ";
            sql += "group by Ordem, Pergunta, Resposta ";
            sql += "order by Ordem, Pergunta, Resposta ";

            var componenteCurricularId = ComponenteCurricularSondagemEnum.Portugues.Name();

            var periodo = $"{ bimestre }° Bimestre";

            var parametros = new { grupoId = grupo.Name(), componenteCurricularId, periodo, dreCodigo, ueCodigo, anoLetivo, anoTurma };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryAsync<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaQueryDto>(sql, parametros);
        }
    }
}
