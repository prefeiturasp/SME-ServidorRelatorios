using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterGrupoMatrizIdPorComponenteCurricularIdQueryHandler : IRequestHandler<ObterGrupoMatrizIdPorComponenteCurricularIdQuery, long>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;

        public ObterGrupoMatrizIdPorComponenteCurricularIdQueryHandler(IComponenteCurricularRepository componenteCurricularRepository)
        {
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository));
        }
        public Task<long> Handle(ObterGrupoMatrizIdPorComponenteCurricularIdQuery request, CancellationToken cancellationToken)
            => componenteCurricularRepository.ObterGrupoMatrizIdPorComponenteCurricularId(request.ComponenteCurricularId);
    }
}
