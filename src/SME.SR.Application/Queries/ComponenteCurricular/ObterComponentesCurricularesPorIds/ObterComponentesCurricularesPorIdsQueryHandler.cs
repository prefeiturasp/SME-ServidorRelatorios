using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
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
        {
            var componentes = await componenteCurricularRepository.ObterDisciplinasPorIds(request.Ids);

            if (componentes == null || !componentes.Any())
                throw new NegocioException($"Não foi possível os detalhes dos componentes informados para obter por ID");

            return componentes;
        }
    }
}
