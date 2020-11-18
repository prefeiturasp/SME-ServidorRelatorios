using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterDrePorIdQuery : IRequest<Dre>
    {
        public ObterDrePorIdQuery(long dreId)
        {
            DreId = dreId;
        }

        public long DreId { get; set; }
    }
}
