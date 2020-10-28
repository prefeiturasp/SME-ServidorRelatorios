using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterDiasPorPeriodosEscolaresComEventosLetivosENaoLetivosQuery : IRequest<List<DiaLetivoDto>>
    {
        public ObterDiasPorPeriodosEscolaresComEventosLetivosENaoLetivosQuery(IEnumerable<PeriodoEscolar> periodosEscolares, long tipoCalendarioId)
        {
            PeriodosEscolares = periodosEscolares;
            TipoCalendarioId = tipoCalendarioId;
        }
        public long TipoCalendarioId { get; set; }
        public IEnumerable<PeriodoEscolar> PeriodosEscolares { get; set; }
    }
}
