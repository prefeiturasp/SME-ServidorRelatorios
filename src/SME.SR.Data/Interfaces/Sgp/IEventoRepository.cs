using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IEventoRepository
    {
        Task<IEnumerable<Evento>> ObterEventosPorTipoCalendarioId(long tipoCalendarioId);
        Task<bool> EhEventoLetivoPorTipoDeCalendarioDataDreUe(long tipoCalendarioId, DateTime data, string dreId, string ueId);
        Task<bool> EhEventoNaoLetivoPorTipoDeCalendarioDataDreUe(long tipoCalendarioId, DateTime data, string dreId, string ueId);
    }
}