using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterUesPorDreIdQuery : IRequest<IEnumerable<Ue>>
    {
        public ObterUesPorDreIdQuery(long dreId)
        {
            DreId = dreId;
        }

        public long DreId { get; set; }
    }
}
