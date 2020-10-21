using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IMathPoolCARepository
    {
        Task<IEnumerable<MathPoolCA>> ObterPorFiltros(string codigoDre, string codigoUe, int anoTurma, int anoLetivo, int semestre);
    }
}
