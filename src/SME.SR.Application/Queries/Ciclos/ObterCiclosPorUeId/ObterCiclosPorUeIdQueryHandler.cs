using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterCiclosPorUeIdQueryHandler : IRequestHandler<ObterCiclosPorUeIdQuery, IEnumerable<TipoCiclo>>
    {
        private readonly ICicloRepository cicloRepository;

        public ObterCiclosPorUeIdQueryHandler(ICicloRepository cicloRepository)
        {
            this.cicloRepository = cicloRepository ?? throw new ArgumentNullException(nameof(cicloRepository)); ;
        }

        public async Task<IEnumerable<TipoCiclo>> Handle(ObterCiclosPorUeIdQuery request, CancellationToken cancellationToken)
        {
            return await cicloRepository.ObterPorUeId(request.UeId);            
        }
    }
}
