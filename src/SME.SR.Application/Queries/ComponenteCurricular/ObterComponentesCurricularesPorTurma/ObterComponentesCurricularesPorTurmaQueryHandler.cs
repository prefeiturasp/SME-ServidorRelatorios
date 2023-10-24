using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesPorTurmaQueryHandler : IRequestHandler<ObterComponentesCurricularesPorTurmaQuery, IEnumerable<ComponenteCurricularPorTurma>>
    {
        private readonly IMediator mediator;

        public ObterComponentesCurricularesPorTurmaQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<ComponenteCurricularPorTurma>> Handle(ObterComponentesCurricularesPorTurmaQuery request, CancellationToken cancellationToken)
            => await mediator.Send(new ObterComponentesCurricularesPorTurmasQuery(new string[] { request.CodigoTurma }));
    }
}
