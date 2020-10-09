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

        public async Task<IEnumerable<PerguntasAutoralDto>> ObterPerguntasPorComponenteAnoTurma(int anoTurma, ComponenteCurricularSondagemEnum? componenteCurricularSondagem)
        {
            StringBuilder query = new StringBuilder();

            query.Append(" select \"AnoEscolar\", p.\"Id\" PerguntaId, p.\"Descricao\" Pergunta, r.\"Id\" RespostaId,  r.\"Descricao\" Resposta ");
            query.Append(" from \"Pergunta\" p ");
            query.Append(" inner join \"PerguntaAnoEscolar\" pae on pae.\"PerguntaId\" = p.\"Id\" ");
            query.Append(" inner join \"PerguntaResposta\" pr on pr.\"PerguntaId\" = p.\"Id\" ");
            query.Append(" inner join \"Resposta\" r on pr.\"RespostaId\" = r.\"Id\"  where 1=1 ");

            if (anoTurma > 0)
                query.Append("and \"AnoEscolar\" = @anoTurma ");

            if (componenteCurricularSondagem != null)
                query.Append("and p.\"ComponenteCurricularId\" = @componenteCurricularId ");

            query.Append("order by \"Ordenacao\" ");

            var parametros = new { anoTurma, componenteCurricularId = componenteCurricularSondagem.Name() };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);
            return await conexao.QueryAsync<PerguntasAutoralDto>(query.ToString(), parametros);
        }
    }
}
