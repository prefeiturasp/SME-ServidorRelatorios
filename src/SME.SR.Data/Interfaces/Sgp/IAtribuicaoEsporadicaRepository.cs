using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IAtribuicaoEsporadicaRepository
    {
        Task<IEnumerable<AtribuicaoEsporadica>> ObterPorFiltros(int anoLetivo, string dreId, string ueId, string codigoRF);
    }
}
