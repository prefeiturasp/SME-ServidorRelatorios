using MediatR;
using SME.SR.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterUeComDrePorIdQueryHandler : IRequestHandler<ObterUeComDrePorIdQuery, Ue>
    {
        private readonly IUeRepository ueRepository;

        public ObterUeComDrePorIdQueryHandler(IUeRepository ueRepository)
        {
            this.ueRepository = ueRepository ?? throw new ArgumentNullException(nameof(ueRepository));
        }

        public async Task<Ue> Handle(ObterUeComDrePorIdQuery request, CancellationToken cancellationToken)
            => await ueRepository.ObterUeComDrePorId(request.UeId);
    }
}
