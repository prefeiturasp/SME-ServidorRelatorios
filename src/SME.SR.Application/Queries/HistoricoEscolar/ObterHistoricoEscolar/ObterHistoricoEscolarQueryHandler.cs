using MediatR;
using SME.SR.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterHistoricoEscolarQueryHandler : IRequestHandler<ObterHistoricoEscolarQuery, IEnumerable<HistoricoEscolarDTO>>
    {
        public ObterHistoricoEscolarQueryHandler()
        {
        }

        public Task<IEnumerable<HistoricoEscolarDTO>> Handle(ObterHistoricoEscolarQuery request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
