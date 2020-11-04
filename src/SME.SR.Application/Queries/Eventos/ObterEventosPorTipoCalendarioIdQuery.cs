using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterEventosPorTipoCalendarioIdQuery : IRequest<IEnumerable<Evento>>
    {
        public ObterEventosPorTipoCalendarioIdQuery(long tipoCalendarioId, DateTime periodoInicio, DateTime periodoFim, long? turmaId)
        {
            TipoCalendarioId = tipoCalendarioId;
            PeriodoInicio = periodoInicio;
            PeriodoFim = periodoFim;            
        }

        public long TipoCalendarioId { get; set; }
        public DateTime PeriodoInicio { get; set; }
        public DateTime PeriodoFim { get; set; }
        public long? TurmaId { get; set; }
    }
}
