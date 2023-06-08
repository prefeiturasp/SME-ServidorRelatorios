using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterHistoricoEscolarQueryHandler : IRequestHandler<ObterHistoricoEscolarQuery, IEnumerable<HistoricoEscolarFundamentalDto>>
    {
        public ObterHistoricoEscolarQueryHandler()
        {
        }

        public Task<IEnumerable<HistoricoEscolarFundamentalDto>> Handle(ObterHistoricoEscolarQuery request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
