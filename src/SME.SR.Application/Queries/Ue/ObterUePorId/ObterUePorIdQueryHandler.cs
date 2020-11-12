using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterUePorIdQueryHandler : IRequestHandler<ObterUePorIdQuery, Ue>
    {
        private readonly IUeRepository ueRepository;

        public ObterUePorIdQueryHandler(IUeRepository ueRepository)
        {
            this.ueRepository = ueRepository ?? throw new ArgumentNullException(nameof(ueRepository));
        }
        public async Task<Ue> Handle(ObterUePorIdQuery request, CancellationToken cancellationToken)
        {
            var dre = await ueRepository.ObterPorId(request.UeId);

            if (dre == null)
            {
                throw new NegocioException("Não foi possível localizar a UE");
            }

            return dre;
        }
    }
}
