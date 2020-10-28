using MediatR;
using SME.SR.Infra;
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
            var datasDosPeriodosEscolares = new List<DiaLetivoDto>();
            foreach (var periodoEscolar in request.PeriodosEscolares.OrderBy(c => c.Bimestre))
            {
                datasDosPeriodosEscolares.AddRange(periodoEscolar.ObterIntervaloDatas().Select(c => new DiaLetivoDto
                {
                    Data = c
                }));
            }

            var eventos = await mediator.Send(new ObterEventosPorTipoCalendarioIdQuery(request.TipoCalendarioId));

            if (eventos != null)
            {
                var datasComEventos = eventos.SelectMany(evento => evento.ObterIntervaloDatas().Select(data => new DiaLetivoDto
                {
                    Data = data,
                    EhLetivo = evento.EhEventoLetivo(),
                    UesIds = string.IsNullOrWhiteSpace(evento.UeId) ? new List<string>() : new List<string> { evento.UeId },
                    PossuiEvento = true
                }));

                datasDosPeriodosEscolares.AddRange(datasComEventos);
            }
            return datasDosPeriodosEscolares;
        }
    }
}
