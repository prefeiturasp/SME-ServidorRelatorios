using MediatR;
using SME.SR.Application.Queries.HistoricoEscolar.ObterHistoricoEscolar;
using SME.SR.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterHistoricoEscolarQueryHandler : IRequestHandler<ObterHistoricoEscolarQuery, IEnumerable<HistoricoEscolar>>
    {
        public ObterHistoricoEscolarQueryHandler()
        {
        }

        public Task<IEnumerable<HistoricoEscolar>> Handle(ObterHistoricoEscolarQuery request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
