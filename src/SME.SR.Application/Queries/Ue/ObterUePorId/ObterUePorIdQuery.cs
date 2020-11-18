using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterUePorIdQuery : IRequest<Ue>
    {
        public ObterUePorIdQuery(long ueId)
        {
            UeId = ueId;
        }

        public long UeId { get; set; }
    }
}
