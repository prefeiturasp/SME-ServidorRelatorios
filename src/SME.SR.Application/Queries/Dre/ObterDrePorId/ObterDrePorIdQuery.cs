using MediatR;
using SME.SR.Data;

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
