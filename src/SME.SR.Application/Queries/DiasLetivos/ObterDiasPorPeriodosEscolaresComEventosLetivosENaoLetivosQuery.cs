using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterDiasPorPeriodosEscolaresComEventosLetivosENaoLetivosQuery : IRequest<List<DiaLetivoDto>>
    {
        public ObterDiasPorPeriodosEscolaresComEventosLetivosENaoLetivosQuery(long tipoCalendarioId)
        {
            TipoCalendarioId = tipoCalendarioId;
        }
        public long TipoCalendarioId { get; set; }
    }
}
