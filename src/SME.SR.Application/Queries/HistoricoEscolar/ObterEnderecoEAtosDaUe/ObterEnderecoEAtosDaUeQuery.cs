using MediatR;

namespace SME.SR.Application
{
    public class ObterEnderecoEAtosDaUeQuery : IRequest<string>
    {
        public string UeCodigo { get; set; }

    }
}
