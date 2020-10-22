using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class VerificaExisteAulaPorTurmaCodigoEComponenteCurricularIdQuery : IRequest<bool>
    {
        public VerificaExisteAulaPorTurmaCodigoEComponenteCurricularIdQuery(long turmaId, string componenteCurricularId, int bimestre, long tipoCalendarioId)
        {
            TurmaId = turmaId;
            Bimestre = bimestre;
            ComponenteCurricularId = componenteCurricularId;
            TipoCalendarioId = tipoCalendarioId;

        }

        public long TurmaId { get; set; }
        public long TipoCalendarioId { get; set; }
        public int Bimestre { get; set; }
        public string ComponenteCurricularId { get; set; }
    }
}
