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
    public class PerguntasAutoralRepository : IPerguntasAutoralRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public PerguntasAutoralRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<PerguntasAutoralDto>> ObterPorFiltros(string codigoDre, string codigoUe, int? anoTurma, int? anoLetivo, ComponenteCurricularSondagemEnum? componenteCurricularSondagem)
        {
            StringBuilder query = new StringBuilder();

            query.Append(" select \"CodigoDre\", \"CodigoUe\", \"CodigoTurma\", \"CodigoAluno\", \"NomeAluno\", \"AnoLetivo\", ");
            query.Append(" \"AnoTurma\", p.\"Id\" PerguntaId, p.\"Descricao\" Pergunta, r.\"Id\" RespostaId,  r.\"Descricao\" Resposta ");
            query.Append(" from \"SondagemAutoral\" sa ");
            query.Append(" inner join \"Pergunta\" p on sa.\"PerguntaId\" = p.\"Id\" ");
            query.Append(" left join \"Resposta\" r on sa.\"RespostaId\" = r.\"Id\"  where 1=1 ");

            if (!string.IsNullOrEmpty(codigoDre))
                query.Append("and \"CodigoDre\" = @codigoDre ");

            if (!string.IsNullOrEmpty(codigoUe))
                query.Append("and \"CodigoUe\" = @codigoUe ");

            if (anoTurma.HasValue && anoTurma > 0)
                query.Append("and \"AnoTurma\" = @anoTurma ");

            if (anoLetivo.HasValue && anoLetivo > 0)
                query.Append("and \"AnoLetivo\" = @anoLetivo ");

            if (componenteCurricularSondagem != null)
                query.Append("and sa.\"ComponenteCurricularId\" = @componenteCurricularId ");

            var parametros = new { codigoDre, codigoUe, anoTurma, anoLetivo, componenteCurricularId = componenteCurricularSondagem.Name() };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);
            return await conexao.QueryAsync<PerguntasAutoralDto>(query.ToString(), parametros);
        }
    }
}
