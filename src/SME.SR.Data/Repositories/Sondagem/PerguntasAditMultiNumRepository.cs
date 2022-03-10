using Npgsql;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data.Repositories.Sondagem
{
    public class PerguntasAditMultiNumRepository : IPerguntasAditMultiNumRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public PerguntasAditMultiNumRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<PerguntasAditMultNumDto>> ObterPerguntasOrdem(int? anoTurma, int? anoLetivo, ComponenteCurricularSondagemEnum? componenteCurricularSondagem, ProficienciaSondagemEnum proficiencia)
        {
            StringBuilder query = new StringBuilder();

            query.Append(" select \"AnoEscolar\", p.\"Id\" Id, p.\"PerguntaId\", p.\"Descricao\" Pergunta");
            query.Append(" from \"Pergunta\" p ");
            query.Append(" inner join \"PerguntaAnoEscolar\" pae on pae.\"PerguntaId\" = p.\"Id\" ");

            if (anoTurma > 0)
                query.Append("and \"AnoEscolar\" = @anoTurma ");

            if (componenteCurricularSondagem != null)
                query.Append("and p.\"ComponenteCurricularId\" = @componenteCurricularId ");

            query.Append("and pae.\"Grupo\" = @proficiencia ");

            query.Append("and ((EXTRACT(YEAR FROM pae.\"InicioVigencia\") <= @anoLetivo and pae.\"FimVigencia\" is null)");

            query.Append("or @anoLetivo <= (case when pae.\"FimVigencia\" is null then 0 else EXTRACT(YEAR FROM pae.\"FimVigencia\") end)) ");

            query.Append("order by pae.\"Ordenacao\"");

            var parametros = new { anoTurma, anoLetivo = anoLetivo, componenteCurricularId = componenteCurricularSondagem.Name(), proficiencia };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);
            return await conexao.QueryAsync<PerguntasAditMultNumDto>(query.ToString(), parametros);
        }

        public async Task<IEnumerable<SondagemAutoralDto>> ObterPorFiltros(string codigoDre, string codigoUe, string grupoId, string periodoId, int bimestre, int? anoTurma, int? anoLetivo, ComponenteCurricularSondagemEnum? componenteCurricularSondagem)
        {
            StringBuilder query = new StringBuilder();

            query.Append(" select \"CodigoDre\", \"CodigoUe\", \"CodigoTurma\", \"CodigoAluno\", \"NomeAluno\", \"AnoLetivo\", ");
            query.Append(" \"AnoTurma\", \"PerguntaId\", \"RespostaId\"");
            query.Append(" from \"Sondagem\" s inner join \"SondagemAluno\" sa on sa.\"SondagemId\" = s.\"Id\" ");
            query.Append(" inner join \"SondagemAlunoRespostas\" sar on sar.\"SondagemAlunoId\" = sa.\"Id\"  ");
            query.Append(" where 1=1 ");

            if (!string.IsNullOrEmpty(codigoDre) && !codigoDre.Equals("0"))
                query.Append("and \"CodigoDre\" = @codigoDre ");

            if (!string.IsNullOrEmpty(codigoUe))
                query.Append("and \"CodigoUe\" = @codigoUe ");

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
                query.Append("and \"ComponenteCurricularId\" = @componenteCurricularId ");

            var parametros = new { codigoDre, codigoUe, grupoId, bimestre, periodoId, anoTurma, anoLetivo, componenteCurricularId = componenteCurricularSondagem.Name() };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);
            return await conexao.QueryAsync<SondagemAutoralDto>(query.ToString(), parametros);
        }

        public async Task<IEnumerable<RespostaDescricaoDto>> ObterRespostasDaPergunta(string perguntaId)
        {
            StringBuilder query = new StringBuilder();

            query.Append(" select r.\"Id\" RespostaId, r.\"Descricao\" Resposta ");
            query.Append(" from \"PerguntaResposta\" pr ");
            query.Append(" inner join \"Resposta\" r on r.\"Id\" = pr.\"RespostaId\" ");
            query.Append(" where pr.\"PerguntaId\" = @perguntaId order by pr.\"Ordenacao\" ");

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);
            return await conexao.QueryAsync<RespostaDescricaoDto>(query.ToString(), new { perguntaId = perguntaId});
        }
    }
}
