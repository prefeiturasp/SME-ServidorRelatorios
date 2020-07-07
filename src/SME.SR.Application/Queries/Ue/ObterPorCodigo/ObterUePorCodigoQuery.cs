using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterUePorCodigoQuery :IRequest<Ue>
    {
        public string UeCodigo { get; set; }
    }
}
