using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    class VerificaExisteMaisAulaCadastradaNoDiaQueryHandler : IRequestHandler<VerificaExisteMaisAulaCadastradaNoDiaQuery, bool>
    {
        private readonly IAulaRepository aulaRepository;

        public VerificaExisteMaisAulaCadastradaNoDiaQueryHandler(IAulaRepository aulaRepository)
        {
            this.aulaRepository = aulaRepository ?? throw new ArgumentNullException(nameof(aulaRepository));
        }

        public async Task<bool> Handle(VerificaExisteMaisAulaCadastradaNoDiaQuery request, CancellationToken cancellationToken) 
            => await aulaRepository.VerificaExisteMaisAulaCadastradaNoDia(request.TurmaId,
                                                    request.ComponenteCurricularId.ToString(),
                                                    request.TipoCalendarioId,
                                                    request.Bimestre);
    }
}
