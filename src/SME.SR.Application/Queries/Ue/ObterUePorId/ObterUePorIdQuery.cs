using MediatR;
using SME.SR.Data;

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
