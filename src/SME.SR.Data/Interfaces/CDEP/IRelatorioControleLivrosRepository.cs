using SME.SR.Infra.CDEP;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IRelatorioControleLivrosRepository
    {
        Task<IEnumerable<AcervoSolicitacaoDto>> ObterRelatorioControleLivros(long[] tiposAcervosPermitidos, string solicitante, string tombo, SituacaoEmprestimo? situacaoEmprestimo, bool? somenteDevolvidos);
    }
}