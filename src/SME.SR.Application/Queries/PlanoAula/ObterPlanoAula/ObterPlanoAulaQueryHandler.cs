using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterPlanoAulaQueryHandler : IRequestHandler<ObterPlanoAulaQuery, PlanoAulaDto>
    {
        private readonly IPlanoAulaRepository planoAulaRepository;
        public ObterPlanoAulaQueryHandler(IPlanoAulaRepository planoAulaRepository)
        {
            this.planoAulaRepository = planoAulaRepository ?? throw new ArgumentNullException(nameof(planoAulaRepository));
        }
        public async Task<PlanoAulaDto> Handle(ObterPlanoAulaQuery request, CancellationToken cancellationToken)
        {
            return await planoAulaRepository.ObterPorId(request.PlanoAulaId);
        }
    }
}
