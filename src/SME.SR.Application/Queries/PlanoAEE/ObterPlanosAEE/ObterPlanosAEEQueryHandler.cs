using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterPlanosAEEQueryHandler : IRequestHandler<ObterPlanosAEEQuery, IEnumerable<PlanosAeeDto>>
    {
        private readonly IPlanoAeeVersaoRepository planoAeeVersaoRepository;

        public ObterPlanosAEEQueryHandler(IPlanoAeeVersaoRepository planoAeeVersaoRepository)
        {
            this.planoAeeVersaoRepository = planoAeeVersaoRepository ?? throw new ArgumentNullException(nameof(planoAeeVersaoRepository));
        }

        public async Task<IEnumerable<PlanosAeeDto>> Handle(ObterPlanosAEEQuery request, CancellationToken cancellationToken)
        {
            return await planoAeeVersaoRepository.ObterPlanoAEE(request.Filtro);
        }
    }
}