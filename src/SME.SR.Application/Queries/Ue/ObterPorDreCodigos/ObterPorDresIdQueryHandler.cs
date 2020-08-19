using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterPorDresIdQueryHandler : IRequestHandler<ObterPorDresIdQuery, IEnumerable<UePorDresIdResultDto>>
    {
        private readonly IUeRepository ueRepository;

        public ObterPorDresIdQueryHandler(IUeRepository ueRepository)
        {
            this.ueRepository = ueRepository ?? throw new ArgumentNullException(nameof(UeRepository));
        }

        public async Task<IEnumerable<UePorDresIdResultDto>> Handle(ObterPorDresIdQuery request, CancellationToken cancellationToken)
        {
            var ues = await ueRepository.ObterPorDresId(request.DresId);
            return ues;
        }
    }
}
