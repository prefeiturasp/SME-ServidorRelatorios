using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterUesPorDreIdQueryHandler : IRequestHandler<ObterUesPorDreIdQuery, IEnumerable<Ue>>
    {
        private readonly IUeRepository ueRepository;

        public ObterUesPorDreIdQueryHandler(IUeRepository ueRepository)
        {
            this.ueRepository = ueRepository ?? throw new ArgumentNullException(nameof(UeRepository));
        }
        public async Task<IEnumerable<Ue>> Handle(ObterUesPorDreIdQuery request, CancellationToken cancellationToken)
        {
            return await ueRepository.ObterPorDreId(request.DreId);


        }
    }
}
