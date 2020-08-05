using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterUePorCodigoQuery :IRequest<Ue>
    {
        public ObterUePorCodigoQuery(string ueCodigo)
        {
            UeCodigo = ueCodigo;
        }

        public string UeCodigo { get; set; }
    }
}
