using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class VerificaExisteAulaPorTurmaCodigoEComponenteCurricularIdQuery : IRequest<bool>
    {
        public VerificaExisteAulaPorTurmaCodigoEComponenteCurricularIdQuery(long turmaId, string componenteCurricularId)
        {
            TurmaId = turmaId;
            ComponenteCurricularId = componenteCurricularId;
        }

        public long TurmaId { get; set; }
        public string ComponenteCurricularId { get; set; }
    }
}
