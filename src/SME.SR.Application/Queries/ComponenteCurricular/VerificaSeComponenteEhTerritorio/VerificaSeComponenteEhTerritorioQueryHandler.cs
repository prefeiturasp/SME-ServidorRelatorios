using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class VerificaSeComponenteEhTerritorioQueryHandler : IRequestHandler<VerificaSeComponenteEhTerritorioQuery, bool>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;

        public VerificaSeComponenteEhTerritorioQueryHandler(IComponenteCurricularRepository componenteCurricularRepository)
        {
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository));
        }
        public async Task<bool> Handle(VerificaSeComponenteEhTerritorioQuery request, CancellationToken cancellationToken)
            => await componenteCurricularRepository.VerificaSeComponenteEhTerritorio(request.ComponenteCurricularId);
    }
}
