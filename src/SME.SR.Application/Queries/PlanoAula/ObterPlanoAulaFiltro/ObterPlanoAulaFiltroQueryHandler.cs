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
    public class ObterPlanoAulaFiltroQueryHandler : IRequestHandler<ObterPlanoAulaFiltroQuery, PlanoAulaDto>
    {
        private readonly IPlanoAulaRepository planoAulaRepository;
        private readonly IMediator mediator;
        public ObterPlanoAulaFiltroQueryHandler(IPlanoAulaRepository planoAulaRepository, IMediator mediator)
        {
            this.planoAulaRepository = planoAulaRepository ?? throw new ArgumentNullException(nameof(planoAulaRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<PlanoAulaDto> Handle(ObterPlanoAulaFiltroQuery request, CancellationToken cancellationToken)
        {
            var planoAula = await planoAulaRepository.ObterPorId(request.PlanoAulaId);

            if (planoAula == null)
                throw new NegocioException("Plano de aula não encontrado.");

            return planoAula;
        }
    }
}
