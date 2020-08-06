using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterCiclosPorUeIdQuery : IRequest<IEnumerable<TipoCiclo>>
    {
        public ObterCiclosPorUeIdQuery(long ueId)
        {
            UeId = ueId;
        }

        public long UeId { get; set; }
    }
}
