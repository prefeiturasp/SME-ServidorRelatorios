using System.Collections.Generic;
using System.Threading.Tasks;
using SME.SR.Infra;

namespace SME.SR.Data.Interfaces
{
    public interface IConceitoValoresRepository
    {
        Task<IEnumerable<ConceitoDto>> ObterDadosLegendaHistoricoEscolar();
    }
}