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
    class ObterAulaReduzidaPorTurmaComponenteEBimestreQueryHandler : IRequestHandler<ObterAulaReduzidaPorTurmaComponenteEBimestreQuery, IEnumerable<AulaReduzidaDto>>
    {
        private readonly IAulaRepository aulaRepository;

        public ObterAulaReduzidaPorTurmaComponenteEBimestreQueryHandler(IAulaRepository aulaRepository)
        {
            this.aulaRepository = aulaRepository ?? throw new ArgumentNullException(nameof(aulaRepository));
        }

        public async Task<IEnumerable<AulaReduzidaDto>> Handle(ObterAulaReduzidaPorTurmaComponenteEBimestreQuery request, CancellationToken cancellationToken)
            => await aulaRepository.ObterQuantidadeAulasReduzido(request.TurmaId,
                                                    request.ComponenteCurricularCodigo.ToString(),
                                                    request.TipoCalendarioId,
                                                    request.Bimestre, request.ProfessorCJ);
    }
}
