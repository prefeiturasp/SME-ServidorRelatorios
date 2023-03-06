using Dapper;
using Npgsql;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.Sondagem;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class SondagemAutoralRepository : ISondagemAutoralRepository
    {
        private const int TERCEIRO_ANO = 3;
        private readonly VariaveisAmbiente variaveisAmbiente;

        public SondagemAutoralRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<SondagemAutoralDto>> ObterPorFiltros(string codigoDre, string codigoUe, string grupoId, string periodoId, int bimestre, int? anoTurma, int? anoLetivo, ComponenteCurricularSondagemEnum? componenteCurricularSondagem)
        {
            StringBuilder query = new StringBuilder();

            query.Append(" select s.\"CodigoDre\", s.\"CodigoUe\", s.\"CodigoTurma\", sa.\"CodigoAluno\", \"NomeAluno\", \"AnoLetivo\", ");
            query.Append(" \"AnoTurma\", \"PerguntaId\", \"RespostaId\"");
            query.Append(" from \"Sondagem\" s inner join \"SondagemAluno\" sa on sa.\"SondagemId\" = s.\"Id\" ");
            query.Append(" inner join \"SondagemAlunoRespostas\" sar on sar.\"SondagemAlunoId\" = sa.\"Id\"  ");
            query.Append(" where 1=1 ");

            if (!string.IsNullOrEmpty(codigoDre) && !codigoDre.Equals("0"))
                query.Append("and s.\"CodigoDre\" = @codigoDre ");

            if (!string.IsNullOrEmpty(codigoUe))
                query.Append("and s.\"CodigoUe\" = @codigoUe ");

            if (!string.IsNullOrEmpty(grupoId))
                query.Append("and s.\"GrupoId\" = @grupoId ");

            if (!string.IsNullOrEmpty(periodoId))
                query.Append("and s.\"PeriodoId\" = @periodoId ");

            if (anoLetivo >= 2022 && bimestre > 0)
                query.Append("and s.\"Bimestre\" = @bimestre ");

            if (anoTurma.HasValue && anoTurma > 0)
                query.Append("and \"AnoTurma\" = @anoTurma ");

            if (anoLetivo.HasValue && anoLetivo > 0)
                query.Append("and \"AnoLetivo\" = @anoLetivo ");

            if (componenteCurricularSondagem != null)
                query.Append("and s.\"ComponenteCurricularId\" = @componenteCurricularId ");

            var parametros = new { codigoDre, codigoUe, grupoId, bimestre, periodoId, anoTurma, anoLetivo, componenteCurricularId = componenteCurricularSondagem.Name() };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);
            return await conexao.QueryAsync<SondagemAutoralDto>(query.ToString(), parametros);
        }

        public async Task<IEnumerable<PerguntasRespostasDTO>> ObterSondagemPerguntaRespostaConsolidadoBimestre(string codigoDre, string codigoUe, int bimestre, int anoTurma, int anoLetivo, string componenteCurricularId)
        {
            var query = new StringBuilder();

            query.AppendLine("SELECT p.\"Id\" AS \"PerguntaId\",");
            query.AppendLine(" p.\"Descricao\" AS \"PerguntaDescricao\",");
            query.AppendLine(" r.\"Id\" AS \"RespostaId\",");
            query.AppendLine(" r.\"Descricao\" AS \"RespostaDescricao\",");
            query.AppendLine(" pa.\"Ordenacao\", pr.\"Ordenacao\",");
            query.AppendLine(" tabela.\"QtdRespostas\"");
            query.AppendLine(" FROM \"Pergunta\" p");
            query.AppendLine(" INNER JOIN \"PerguntaAnoEscolar\" pa ON pa.\"PerguntaId\" = p.\"Id\"");
            query.AppendLine(" LEFT JOIN \"PerguntaAnoEscolarBimestre\" paeb on pa.\"Id\" = paeb.\"PerguntaAnoEscolarId\"  ");
            query.AppendLine(" INNER JOIN \"PerguntaResposta\" pr ON pr.\"PerguntaId\" = p.\"Id\"");
            query.AppendLine(" INNER JOIN \"Resposta\" r ON r.\"Id\" = pr.\"RespostaId\"");
            query.AppendLine(" LEFT JOIN ( ");
            query.AppendLine("    SELECT p.\"Id\" AS \"PerguntaId\",");
            query.AppendLine("           r.\"Id\" AS \"RespostaId\", COUNT(distinct sa.\"CodigoAluno\") AS \"QtdRespostas\"");
            query.AppendLine("    FROM \"SondagemAlunoRespostas\" sar");
            query.AppendLine("    INNER JOIN \"SondagemAluno\" sa ON sa.\"Id\" = sar.\"SondagemAlunoId\"");
            query.AppendLine("    INNER JOIN \"Sondagem\" s ON s.\"Id\" = sa.\"SondagemId\"");
            query.AppendLine("    INNER JOIN \"Pergunta\" p ON p.\"Id\" = sar.\"PerguntaId\"");
            query.AppendLine("    INNER JOIN \"Resposta\" r ON r.\"Id\" = sar.\"RespostaId\"");
            query.AppendLine("    WHERE s.\"ComponenteCurricularId\" = @componenteCurricularId");
            query.AppendLine("      AND s.\"AnoLetivo\" = @anoLetivo");
            query.AppendLine("      AND s.\"AnoTurma\" = @anoTurma");
            query.AppendLine("      AND s.\"Bimestre\" = @bimestre");

            if (!string.IsNullOrEmpty(codigoDre))
                query.AppendLine(" AND s.\"CodigoDre\" =  @codigoDre");
            if (!string.IsNullOrEmpty(codigoUe))
                query.AppendLine(" AND s.\"CodigoUe\" =  @codigoUe");

            query.AppendLine("    GROUP BY p.\"Id\", r.\"Id\") AS tabela");
            query.AppendLine(" ON p.\"Id\" = tabela.\"PerguntaId\" AND r.\"Id\"= tabela.\"RespostaId\"");
            query.AppendLine(" WHERE ((pa.\"FimVigencia\" IS NULL AND EXTRACT (YEAR FROM pa.\"InicioVigencia\") <= @anoLetivo)");
            query.AppendLine("    OR (EXTRACT(YEAR FROM pa.\"FimVigencia\") >= @anoLetivo AND EXTRACT (YEAR FROM pa.\"InicioVigencia\") <= @anoLetivo))");
            query.AppendLine("   AND pa.\"AnoEscolar\" = @anoTurma");

            query.AppendLine("   AND (paeb.\"Id\" is null");
            query.AppendLine("   AND NOT EXISTS (SELECT 1 FROM \"PerguntaAnoEscolar\" pae");
            query.AppendLine("                   INNER JOIN  \"PerguntaAnoEscolarBimestre\" paeb ON paeb.\"PerguntaAnoEscolarId\" = pae.\"Id\"");
            query.AppendLine("                   WHERE pae.\"AnoEscolar\" = @anoTurma");
            query.AppendLine("                     AND (pae.\"FimVigencia\" IS NULL AND EXTRACT(YEAR FROM pae.\"InicioVigencia\") <= @anoLetivo)");
            query.AppendLine("                     AND paeb.\"Bimestre\" = @bimestre)");
            query.AppendLine("    OR paeb.\"Bimestre\" = @bimestre)");

            if (anoTurma <= TERCEIRO_ANO)
                query.AppendLine(" AND pa.\"Grupo\" = " + (int)ProficienciaSondagemEnum.Numeros);

            query.AppendLine(" ORDER BY pa.\"Ordenacao\", pr.\"Ordenacao\", p.\"Descricao\", r.\"Descricao\"");


            var parametros = new { codigoDre, codigoUe, bimestre, anoTurma, anoLetivo, componenteCurricularId };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);
            return await conexao.QueryAsync<PerguntasRespostasDTO>(query.ToString(), parametros);
        }
    }
}
