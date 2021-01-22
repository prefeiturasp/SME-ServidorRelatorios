using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterEventosPorTipoCalendarioIdEPeriodoInicioEFimQuery : IRequest<IEnumerable<Evento>>
    {
        public ObterEventosPorTipoCalendarioIdEPeriodoInicioEFimQuery(long tipoCalendarioId, DateTime periodoInicio, DateTime periodoFim, long? turmaId = null)
        {
            TipoCalendarioId = tipoCalendarioId;
            PeriodoInicio = periodoInicio;
            PeriodoFim = periodoFim;
            TurmaId = turmaId;
        }


        public DateTime PeriodoInicio { get; set; }
        public DateTime PeriodoFim { get; set; }
        public long TipoCalendarioId { get; set; }
        public long? TurmaId { get; set; }
    }
}