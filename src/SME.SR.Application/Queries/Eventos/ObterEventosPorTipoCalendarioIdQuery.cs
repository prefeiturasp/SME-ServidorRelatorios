using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterEventosPorTipoCalendarioIdQuery : IRequest<IEnumerable<Evento>>
    {
        public ObterEventosPorTipoCalendarioIdQuery(long tipoCalendarioId)
        {
            TipoCalendarioId = tipoCalendarioId;
        }

        public long TipoCalendarioId { get; set; }
    }
}
