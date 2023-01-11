using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterPlanoAEEPorVersaoPlanoIdQueryHandler : IRequestHandler<ObterPlanoAEEPorVersaoPlanoIdQuery, PlanoAeeDto>
    {
        private readonly IPlanoAeeVersaoRepository planoAeeVersaoRepository;

        public ObterPlanoAEEPorVersaoPlanoIdQueryHandler(IPlanoAeeVersaoRepository planoAeeVersaoRepository)
        {
            this.planoAeeVersaoRepository = planoAeeVersaoRepository ?? throw new ArgumentNullException(nameof(planoAeeVersaoRepository));
        }

        public async Task<PlanoAeeDto> Handle(ObterPlanoAEEPorVersaoPlanoIdQuery request, CancellationToken cancellationToken)
        {
            return await planoAeeVersaoRepository.ObterPlanoAeePorVersaoPlanoId(request.VersaoPlanoId);
        }
    }
}