using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterDrePorCodigoQuery : IRequest<Dre>
    {
        public string DreCodigo { get; set; }
    }
}
