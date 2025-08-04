using SME.SR.Infra.CDEP;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces.CDEP
{
    public interface IRelatorioControleLivrosRepository
    {
        Task<IEnumerable<AcervoSolicitacaoDto>> ObterRelatorioControleLivrosSintetico(SituacaoSolicitacaoItem situacaoSolicitacaoItem);
    }
}
