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
    public class ObterPlanoAulaObjetivoAprendizagemQueryHandler : IRequestHandler<ObterPlanoAulaObjetivoAprendizagemQuery, IEnumerable<ObjetivoAprendizagemDto>>
    {
        private readonly IPlanoAulaRepository planoAulaRepository;
        public ObterPlanoAulaObjetivoAprendizagemQueryHandler(IPlanoAulaRepository planoAulaRepository)
        {
            this.planoAulaRepository = planoAulaRepository ?? throw new ArgumentNullException(nameof(planoAulaRepository));
        }
        public async Task<IEnumerable<ObjetivoAprendizagemDto>> Handle(ObterPlanoAulaObjetivoAprendizagemQuery request, CancellationToken cancellationToken)
        {
            return await planoAulaRepository.ObterObjetivoAprendizagemPorPlanoAulaId(request.PlanoAulaId);
        }
    }
}
