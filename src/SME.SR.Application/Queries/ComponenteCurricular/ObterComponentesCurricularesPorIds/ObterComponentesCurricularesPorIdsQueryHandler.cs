using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesPorIdsQueryHandler : IRequestHandler<ObterComponentesCurricularesPorIdsQuery, IEnumerable<DisciplinaDto>>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;

        public ObterComponentesCurricularesPorIdsQueryHandler(IComponenteCurricularRepository componenteCurricularSgpRepository)
        {
            this.componenteCurricularRepository = componenteCurricularSgpRepository ?? throw new ArgumentNullException(nameof(componenteCurricularSgpRepository));
        }

        public async Task<IEnumerable<DisciplinaDto>> Handle(ObterComponentesCurricularesPorIdsQuery request, CancellationToken cancellationToken)
                => await componenteCurricularRepository.ObterDisciplinasPorIds(request.Ids);
    }
}
