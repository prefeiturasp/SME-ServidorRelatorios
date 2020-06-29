using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterPeriodosEscolaresPorTipoCalendarioQuery: IRequest<IEnumerable<PeriodoEscolar>>
    {
        public ObterPeriodosEscolaresPorTipoCalendarioQuery(long tipoCalendarioId)
        {
            TipoCalendarioId = tipoCalendarioId;
        }

        public long TipoCalendarioId { get; set; }
    }
}
