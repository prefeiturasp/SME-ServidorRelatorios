using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IAreaDoConhecimentoRepository
    {
        Task<IEnumerable<AreaDoConhecimento>> ObterAreasDoConhecimentoPorComponentesCurriculares(long[] codigosComponentesCurriculares);
    }
}
