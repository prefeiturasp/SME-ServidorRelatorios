﻿using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterDiasPorPeriodosEscolaresComEventosLetivosENaoLetivosQuery : IRequest<List<DiaLetivoDto>>
    {
        public ObterDiasPorPeriodosEscolaresComEventosLetivosENaoLetivosQuery(long tipoCalendarioId, DateTime periodoInicio, DateTime periodoFim, long? turmaId = null)
        {
            PeriodoInicio = periodoInicio;
            PeriodoFim = periodoFim;
            TipoCalendarioId = tipoCalendarioId;
            TurmaId = turmaId;
        }

        public DateTime PeriodoInicio { get; set; }
        public DateTime PeriodoFim { get; set; }
        public long TipoCalendarioId { get; set; }
        public long? TurmaId { get; set; }
    }
}
