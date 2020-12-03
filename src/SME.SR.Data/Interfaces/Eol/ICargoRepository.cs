using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface ICargoRepository
    {
        Task<IEnumerable<ServidorCargoDto>> BuscaCargosRfPorAnoLetivo(string[] codigosRF, int anoLetivo);
    }
}
