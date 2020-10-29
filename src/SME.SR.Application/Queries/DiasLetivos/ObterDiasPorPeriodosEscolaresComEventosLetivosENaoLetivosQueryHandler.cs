using MediatR;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDiasPorPeriodosEscolaresComEventosLetivosENaoLetivosQueryHandler : IRequestHandler<ObterDiasPorPeriodosEscolaresComEventosLetivosENaoLetivosQuery, List<DiaLetivoDto>>
    {
        private readonly IMediator mediator;

        public ObterDiasPorPeriodosEscolaresComEventosLetivosENaoLetivosQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<List<DiaLetivoDto>> Handle(ObterDiasPorPeriodosEscolaresComEventosLetivosENaoLetivosQuery request, CancellationToken cancellationToken)
        {
            var DiasLetivos = new List<DiaLetivoDto>();            

            var eventos = await mediator.Send(new ObterEventosPorTipoCalendarioIdQuery(request.TipoCalendarioId, request.PeriodoInicio, request.PeriodoFim));

            if (eventos != null)
            {
                var datasComEventos = eventos.SelectMany(evento => evento.ObterIntervaloDatas().Select(data => new DiaLetivoDto
                {
                    Data = data,
                    Motivo = evento.TipoEvento.Name(),
                    EhLetivo = evento.EhEventoLetivo(),
                    EhNaoLetivo = evento.NaoEhEventoLetivo(),
                    UesIds = string.IsNullOrWhiteSpace(evento.UeId) ? new List<string>() : new List<string> { evento.UeId },
                    PossuiEvento = true
                }));

                DiasLetivos.AddRange(datasComEventos);
            }
            return DiasLetivos;
        }
    }
}
