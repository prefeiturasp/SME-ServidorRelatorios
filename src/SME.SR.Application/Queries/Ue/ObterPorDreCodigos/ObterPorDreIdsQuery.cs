using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterPorDresIdQuery : IRequest<IEnumerable<UePorDresIdResultDto>>
    {
        public ObterPorDresIdQuery(long[] dresId)
        {
            DresId = dresId;
        }

        public long[] DresId { get; set; }
    }
}
