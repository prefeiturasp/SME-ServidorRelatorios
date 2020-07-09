using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterLeisPorModalidadeAnoQueryHandler : IRequestHandler<ObterLeisPorModalidadeAnoQuery, string>
    {

        public ObterLeisPorModalidadeAnoQueryHandler()
        {
        }

        public async Task<string> Handle(ObterLeisPorModalidadeAnoQuery request, CancellationToken cancellationToken)
        {
            return "";
        }
    }
}
