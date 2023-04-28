using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterComponentesTerritorioSaberPorTurmaEComponentesIdsQueryHandler : IRequestHandler<ObterComponentesTerritorioSaberPorTurmaEComponentesIdsQuery, IEnumerable<ComponenteCurricularTerritorioSaber>>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;

        public ObterComponentesTerritorioSaberPorTurmaEComponentesIdsQueryHandler(IComponenteCurricularRepository componenteCurricularRepository)
        {
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository));
        }
        public async Task<IEnumerable<ComponenteCurricularTerritorioSaber>> Handle(ObterComponentesTerritorioSaberPorTurmaEComponentesIdsQuery request, CancellationToken cancellationToken)
            => await componenteCurricularRepository.ObterComponentesTerritorioDosSaberes(request.TurmaCodigo, request.ComponentesIds.ToList());
    }
}
