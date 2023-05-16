using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class VerificaSeComponenteEhTerritorioQuery : IRequest<bool>
    {
        public long ComponenteCurricularId { get; set; }

        public VerificaSeComponenteEhTerritorioQuery(long componenteCurricularId)
        {
            ComponenteCurricularId = componenteCurricularId;
        }
    }
}
