using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterObjetivosAprendizagemPlanejamentoAnualQueryHandler : IRequestHandler<ObterObjetivosAprendizagemPlanejamentoAnualQuery, IEnumerable<BimestreDescricaoObjetivosPlanoAnualDto>>
    {
        private readonly IPlanoAnualRepository planoAnualRepository;
        
        public ObterObjetivosAprendizagemPlanejamentoAnualQueryHandler(IPlanoAnualRepository planoAnualRepository)
        {
            this.planoAnualRepository = planoAnualRepository ?? throw new ArgumentNullException(nameof(planoAnualRepository));
        }
        public async Task<IEnumerable<BimestreDescricaoObjetivosPlanoAnualDto>> Handle(ObterObjetivosAprendizagemPlanejamentoAnualQuery request, CancellationToken cancellationToken)
        {
            return await planoAnualRepository.ObterObjetivosPorPlanoAulaId(request.Id);
        }
    }
}
