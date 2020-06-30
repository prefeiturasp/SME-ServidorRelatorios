using MediatR;
using SME.SR.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterUePorCodigoQueryHandler : IRequestHandler<ObterUePorCodigoQuery, Ue>
    {
        private readonly IUeRepository ueRepository;

        public ObterUePorCodigoQueryHandler(IUeRepository ueRepository)
        {
            this.ueRepository = ueRepository ?? throw new ArgumentNullException(nameof(UeRepository));
        }
        public async Task<Ue> Handle(ObterUePorCodigoQuery request, CancellationToken cancellationToken)
        {
            return await ueRepository.ObterPorCodigo(request.UeCodigo);
        }
    }
}
