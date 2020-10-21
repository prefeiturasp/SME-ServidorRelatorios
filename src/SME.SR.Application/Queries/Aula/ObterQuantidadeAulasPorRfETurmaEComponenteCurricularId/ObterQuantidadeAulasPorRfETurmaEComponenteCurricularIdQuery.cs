using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterQuantidadeAulasPorRfETurmaEComponenteCurricularIdQuery : IRequest<int>
    {
        public ObterQuantidadeAulasPorRfETurmaEComponenteCurricularIdQuery(long turmaId, string componenteCurricularId, string codigoRF)
        {
            TurmaId = turmaId;
            ComponenteCurricularId = componenteCurricularId;
            CodigoRF = codigoRF;
        }

        public long TurmaId { get; set; }
        public string ComponenteCurricularId { get; set; }
        public string CodigoRF { get; set; }
    }
}
