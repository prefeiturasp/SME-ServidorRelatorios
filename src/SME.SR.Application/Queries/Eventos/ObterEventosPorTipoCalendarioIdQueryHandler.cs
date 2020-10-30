using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterEventosPorTipoCalendarioIdQueryHandler : IRequestHandler<ObterEventosPorTipoCalendarioIdQuery, IEnumerable<Evento>>
    {
        private readonly IEventoRepository eventoRepository;
        public ObterEventosPorTipoCalendarioIdQueryHandler(IEventoRepository eventoRepository)
        {
            this.eventoRepository = eventoRepository ?? throw new ArgumentNullException(nameof(eventoRepository));
        }

        public Task<IEnumerable<Evento>> Handle(ObterEventosPorTipoCalendarioIdQuery request, CancellationToken cancellationToken)
                => eventoRepository.ObterEventosPorTipoCalendarioId(request.TipoCalendarioId, request.PeriodoInicio, request.PeriodoFim);
    }
}
