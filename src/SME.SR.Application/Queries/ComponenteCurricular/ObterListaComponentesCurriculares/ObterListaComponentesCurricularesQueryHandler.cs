using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterListaComponentesCurricularesQueryHandler : IRequestHandler<ObterListaComponentesCurricularesQuery, IEnumerable<ComponenteCurricular>>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;

        public ObterListaComponentesCurricularesQueryHandler(IComponenteCurricularRepository componenteCurricularRepository)
        {
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository));
        }

        public async Task<IEnumerable<ComponenteCurricular>> Handle(ObterListaComponentesCurricularesQuery request, CancellationToken cancellationToken)
            => (await componenteCurricularRepository.ListarInformacoesPedagogicasComponentesCurriculares()).ToComponentesCurriculares();
    }
}
