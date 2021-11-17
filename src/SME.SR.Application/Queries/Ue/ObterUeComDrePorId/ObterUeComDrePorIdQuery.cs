using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterUeComDrePorIdQuery : IRequest<Ue>
    {
        public ObterUeComDrePorIdQuery(long ueId)
        {
            UeId = ueId;
        }

        public long UeId { get; set; }
    }
}
