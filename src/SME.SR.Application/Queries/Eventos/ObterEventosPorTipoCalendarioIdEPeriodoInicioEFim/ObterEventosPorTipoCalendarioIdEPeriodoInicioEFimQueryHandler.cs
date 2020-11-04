using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterEventosPorTipoCalendarioIdEPeriodoInicioEFimQueryHandler : IRequestHandler<ObterEventosPorTipoCalendarioIdEPeriodoInicioEFimQuery, IEnumerable<Evento>>
    {
        private readonly IEventoRepository eventoRepository;
        public ObterEventosPorTipoCalendarioIdEPeriodoInicioEFimQueryHandler(IEventoRepository eventoRepository)
        {
            this.eventoRepository = eventoRepository ?? throw new ArgumentNullException(nameof(eventoRepository));
        }

        public Task<IEnumerable<Evento>> Handle(ObterEventosPorTipoCalendarioIdEPeriodoInicioEFimQuery request, CancellationToken cancellationToken)
        {
            return eventoRepository.ObterEventosPorTipoCalendarioIdEPeriodoInicioEFim(request.TipoCalendarioId, request.PeriodoInicio, request.PeriodoFim, request.TurmaId);
        }
    }
}