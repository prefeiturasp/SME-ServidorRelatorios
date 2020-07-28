using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IParecerFinalRepository
    {
        Task<IEnumerable<RelatorioParecerConclusivoRetornoDto>> ObterPareceresFinais(int anoLetivo, string dreCodigo, string ueCodigo, long modalidadeId, int? semestre,
                                                                                                            long cicloId, string[] anos, long parecerConclusivoId);
    }
}
