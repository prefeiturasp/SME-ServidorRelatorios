using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterCicloPorIdQuery : IRequest<TipoCiclo>
    {
        public long CicloId { get; set; }
    }
}
