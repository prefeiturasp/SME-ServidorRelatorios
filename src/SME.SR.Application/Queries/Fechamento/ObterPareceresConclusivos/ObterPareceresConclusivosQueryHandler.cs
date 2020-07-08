using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterPareceresConclusivosQueryHandler : IRequestHandler<ObterPareceresConclusivosQuery, long[]>
    {
        

        public ObterPareceresConclusivosQueryHandler()
        {
        
        }
        public async Task<long[]> Handle(ObterPareceresConclusivosQuery request, CancellationToken cancellationToken)
        {

            return default;
        }
    }
}
