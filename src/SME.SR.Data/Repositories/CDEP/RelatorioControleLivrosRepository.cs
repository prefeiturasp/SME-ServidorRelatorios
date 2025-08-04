using Npgsql;
using SME.SR.Data.Interfaces.CDEP;
using SME.SR.Infra;
using SME.SR.Infra.CDEP;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data.Repositories.CDEP
{
    public class RelatorioControleLivrosRepository : IRelatorioControleLivrosRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public RelatorioControleLivrosRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<AcervoSolicitacaoDto>> ObterRelatorioControleLivrosSintetico(SituacaoSolicitacaoItem situacaoSolicitacaoItem)
        {
            var query = new StringBuilder();
            query.AppendLine(@"
                    SELECT 
                         a.titulo
                    FROM acervo_solicitacao_item asi
                       JOIN acervo_solicitacao aso on aso.id = asi.acervo_solicitacao_id
                       JOIN acervo a on a.id = asi.acervo_id
                       JOIN usuario u on u.id = aso.usuario_id
                       LEFT JOIN usuario ur on ur.id = asi.usuario_responsavel_id and not ur.excluido
                       LEFT JOIN LATERAL (
                           SELECT situacao
                           FROM acervo_emprestimo ae
                           WHERE ae.acervo_solicitacao_item_id = asi.id
                           AND not ae.excluido
                           ORDER BY ae.id DESC
                           LIMIT 1
                       ) situacao_emprestimo ON true 
                    where not asi.excluido
                      and not aso.excluido
                      and not a.excluido 
                      and not u.excluido 
                      and a.tipo = ANY(@tiposAcervosPermitidos) ");

            var parametros = new { };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);
            return await conexao.QueryAsync<AcervoSolicitacaoDto>(query.ToString(), parametros);
        }
    }
}
