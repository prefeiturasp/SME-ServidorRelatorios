using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterEnderecoEAtosDaUeQuery : IRequest<CabecalhoDto>
    {
        public string UeCodigo { get; set; }

    }
}
