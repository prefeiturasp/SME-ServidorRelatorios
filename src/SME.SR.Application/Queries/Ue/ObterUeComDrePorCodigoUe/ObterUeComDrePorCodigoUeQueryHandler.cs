using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterUeComDrePorCodigoUeQueryHandler : IRequestHandler<ObterUeComDrePorCodigoUeQuery, Ue>
    {
        private readonly IUeRepository ueRepository;

        public ObterUeComDrePorCodigoUeQueryHandler(IUeRepository ueRepository)
        {
            this.ueRepository = ueRepository ?? throw new ArgumentNullException(nameof(ueRepository));
        }

        public async Task<Ue> Handle(ObterUeComDrePorCodigoUeQuery request, CancellationToken cancellationToken)
        {
            return await ueRepository.ObterUeComDrePorCodigo(request.UeCodigo);
        }
    }
}