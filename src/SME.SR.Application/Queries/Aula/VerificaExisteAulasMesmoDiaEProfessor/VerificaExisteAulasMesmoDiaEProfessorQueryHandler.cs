using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class VerificaExisteAulasMesmoDiaEProfessorQueryHandler : IRequestHandler<VerificaExisteAulasMemoDiaEProfessorQuery, bool>
    {
        private readonly IAulaRepository aulaRepository;

        public VerificaExisteAulasMesmoDiaEProfessorQueryHandler(IAulaRepository aulaRepository)
        {
            this.aulaRepository = aulaRepository ?? throw new ArgumentNullException(nameof(aulaRepository));
        }

        public async Task<bool> Handle(VerificaExisteAulasMemoDiaEProfessorQuery request, CancellationToken cancellationToken)
            => await aulaRepository.VerificaExisteAulasMesmoDiaProfessor(request.TurmaId, request.ComponenteCurricularId, request.TipoCalendarioId, request.Bimestre);
    }
}
