using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    class VerificaExisteAulaPorTurmaCodigoEComponenteCurricularIdQueryHandler : IRequestHandler<VerificaExisteAulaPorTurmaCodigoEComponenteCurricularIdQuery, bool>
    {
        private readonly IAulaRepository aulaRepository;

        public VerificaExisteAulaPorTurmaCodigoEComponenteCurricularIdQueryHandler(IAulaRepository aulaRepository)
        {
            this.aulaRepository = aulaRepository ?? throw new ArgumentNullException(nameof(aulaRepository));
        }

        public async Task<bool> Handle(VerificaExisteAulaPorTurmaCodigoEComponenteCurricularIdQuery request, CancellationToken cancellationToken)
         => await aulaRepository.VerificaExisteAulaCadastrada(request.TurmaId, request.ComponenteCurricularId, request.Bimestre, request.TipoCalendarioId);
    }
}
