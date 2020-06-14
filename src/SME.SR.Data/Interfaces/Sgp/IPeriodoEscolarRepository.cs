using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IPeriodoEscolarRepository
    {
        Task<IEnumerable<PeriodoEscolar>> ObterPeriodosEscolaresPorTipoCalendario(long tipoCalendarioId);
    }
}
