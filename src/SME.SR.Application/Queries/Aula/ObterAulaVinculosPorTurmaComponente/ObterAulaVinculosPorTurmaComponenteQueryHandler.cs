using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAulaVinculosPorTurmaComponenteQueryHandler : IRequestHandler<ObterAulaVinculosPorTurmaComponenteQuery, IEnumerable<AulaVinculosDto>>
    {
        private readonly IAulaRepository aulaRepository;

        public ObterAulaVinculosPorTurmaComponenteQueryHandler(IAulaRepository aulaRepository)
        {
            this.aulaRepository = aulaRepository ?? throw new ArgumentNullException(nameof(aulaRepository));
        }

        public async Task<IEnumerable<AulaVinculosDto>> Handle(ObterAulaVinculosPorTurmaComponenteQuery request, CancellationToken cancellationToken)
            => await aulaRepository.ObterAulasVinculos(request.TurmasId,
                                                    request.ComponenteCurricularesCodigo,
                                                    request.ProfessorCJ);
    }
}
