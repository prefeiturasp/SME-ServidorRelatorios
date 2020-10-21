using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class VerificaExisteAulaTitularECjQuery : IRequest<bool>
    {
        public VerificaExisteAulaTitularECjQuery(long turmaId, long componenteCurricularId, long tipoCalendarioId, int bimestre)
        {
            TurmaId = turmaId;
            ComponenteCurricularId = componenteCurricularId;
            TipoCalendarioId = tipoCalendarioId;
            Bimestre = bimestre;
        }

        public long TurmaId { get; set; }
        public long ComponenteCurricularId { get; set; }
        public long TipoCalendarioId { get; set; }
        public int Bimestre { get; set; }
    }
}
