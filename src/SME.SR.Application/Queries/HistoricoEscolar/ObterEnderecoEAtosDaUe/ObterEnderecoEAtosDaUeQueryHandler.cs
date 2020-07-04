using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterEnderecoEAtosDaUeQueryHandler : IRequestHandler<ObterEnderecoEAtosDaUeQuery, string>
    {

        public ObterEnderecoEAtosDaUeQueryHandler()
        {
        }

        public async Task<string> Handle(ObterEnderecoEAtosDaUeQuery request, CancellationToken cancellationToken)
        {
            return "";
        }
    }
}
