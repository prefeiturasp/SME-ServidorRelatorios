using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.Aula.ObterAulaReduzidaPorTurmaComponente
{
    public class ObterAulaReduzidaPorTurmaComponenteQueryHandler : IRequestHandler<ObterAulaReduzidaPorTurmaComponenteQuery, IEnumerable<AulaReduzidaDto>>
    {
        private readonly IAulaRepository aulaRepository;

        public ObterAulaReduzidaPorTurmaComponenteQueryHandler(IAulaRepository aulaRepository)
        {
            this.aulaRepository = aulaRepository ?? throw new ArgumentNullException(nameof(aulaRepository));
        }

        public async Task<IEnumerable<AulaReduzidaDto>> Handle(ObterAulaReduzidaPorTurmaComponenteQuery request, CancellationToken cancellationToken)
            => await aulaRepository.ObterAulasReduzido(request.TurmasId,
                                                    request.ComponenteCurricularesCodigo,
                                                    request.ProfessorCJ);
    }
}
