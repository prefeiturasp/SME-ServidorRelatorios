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
    public class SondagemAutoralRepository : ISondagemAutoralRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public SondagemAutoralRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<SondagemAutoralDto>> ObterPorFiltros(string codigoDre, string codigoUe, int? anoTurma, int? anoLetivo, ComponenteCurricularSondagemEnum? componenteCurricularSondagem)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.Append(" select \"CodigoDre\", \"CodigoUe\", \"CodigoTurma\", \"CodigoAluno\", \"NomeAluno\", \"AnoLetivo\", ");
                query.Append(" \"AnoTurma\", \"PerguntaId\", \"RespostaId\"");
                query.Append(" from \"Sondagem\" s inner join \"SondagemAluno\" sa on sa.\"SondagemId\" = s.\"Id\" ");
                query.Append(" inner join \"SondagemAlunoRespostas\" sar on sar.\"SondagemAlunoId\" = sa.\"Id\"  ");
                query.Append(" where 1=1 ");

                if (!string.IsNullOrEmpty(codigoDre))
                    query.Append("and \"CodigoDre\" = @codigoDre ");

                if (!string.IsNullOrEmpty(codigoUe))
                    query.Append("and \"CodigoUe\" = @codigoUe ");

                if (anoTurma.HasValue && anoTurma > 0)
                    query.Append("and \"AnoTurma\" = @anoTurma ");

                if (anoLetivo.HasValue && anoLetivo > 0)
                    query.Append("and \"AnoLetivo\" = @anoLetivo ");

                if (componenteCurricularSondagem != null)
                    query.Append("and \"ComponenteCurricularId\" = @componenteCurricularId ");

                var parametros = new { codigoDre, codigoUe, anoTurma, anoLetivo, componenteCurricularId = componenteCurricularSondagem.Name() };

                using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);
                return await conexao.QueryAsync<SondagemAutoralDto>(query.ToString(), parametros);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
