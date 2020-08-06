using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IParecerConclusivoRepository
    {
        Task<IEnumerable<RelatorioParecerConclusivoRetornoDto>> ObterPareceresFinais(int anoLetivo, string dreCodigo, string ueCodigo, Modalidade? modalidade, int? semestre,
                                                                                                              long cicloId, string[] anos, long parecerConclusivoId);
    }
}
