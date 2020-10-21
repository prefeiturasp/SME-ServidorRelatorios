using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class VerificaExisteAulaTitularECjQueryHandler : IRequestHandler<VerificaExisteAulaTitularECjQuery, bool>
    {
        private readonly IAulaRepository aulaRepository;

        public VerificaExisteAulaTitularECjQueryHandler(IAulaRepository aulaRepository)
        {
            this.aulaRepository = aulaRepository ?? throw new ArgumentNullException(nameof(aulaRepository));
        }

        public async Task<bool> Handle(VerificaExisteAulaTitularECjQuery request, CancellationToken cancellationToken)
            => await aulaRepository.VerificaExsiteAulaTitularECj(request.TurmaId, request.ComponenteCurricularId, request.TipoCalendarioId, request.Bimestre);
    }
}
