using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IEventoRepository
    {
        Task<IEnumerable<Evento>> ObterEventosPorTipoCalendarioIdEPeriodoInicioEFim(long tipoCalendarioId, DateTime periodoInicio, DateTime periodoFim, long? turmaId = null);
        Task<bool> EhEventoLetivoPorTipoDeCalendarioDataDreUe(long tipoCalendarioId, DateTime data, string dreId, string ueId);
        Task<bool> EhEventoNaoLetivoPorTipoDeCalendarioDataDreUe(long tipoCalendarioId, DateTime data, string dreId, string ueId);
        Task<IEnumerable<Evento>> ObterEventosPorTipoDeCalendarioAsync(long tipoCalendarioId, string ueCodigo = "", params EventoLetivo[] tiposLetivosConsiderados);
    }
}