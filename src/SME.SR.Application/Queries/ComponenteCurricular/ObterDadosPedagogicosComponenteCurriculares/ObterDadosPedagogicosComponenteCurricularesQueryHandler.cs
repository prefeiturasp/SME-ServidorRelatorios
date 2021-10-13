using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application 
{ 
    public class ObterDadosPedagogicosComponenteCurricularesQueryHandler : IRequestHandler<ObterDadosPedagogicosComponenteCurricularesQuery, IEnumerable<ComponenteCurricularPedagogicoDto>>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;

        public ObterDadosPedagogicosComponenteCurricularesQueryHandler(IComponenteCurricularRepository componenteCurricularSgpRepository)
        {
            this.componenteCurricularRepository = componenteCurricularSgpRepository ?? throw new ArgumentNullException(nameof(componenteCurricularSgpRepository));
        }

        public async Task<IEnumerable<ComponenteCurricularPedagogicoDto>> Handle(ObterDadosPedagogicosComponenteCurricularesQuery request, CancellationToken cancellationToken)
                => null;// await componenteCurricularRepository.ObterDisciplinasPorIds(request.Ids);
    }
}
