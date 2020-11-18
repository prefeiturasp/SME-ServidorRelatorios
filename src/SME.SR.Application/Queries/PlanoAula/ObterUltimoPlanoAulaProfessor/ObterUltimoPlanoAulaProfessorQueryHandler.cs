using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterUltimoPlanoAulaProfessorQueryHandler : IRequestHandler<ObterUltimoPlanoAulaProfessorQuery, DateTime>
    {
        private readonly IPlanoAulaRepository planoAulaRepository;
        public ObterUltimoPlanoAulaProfessorQueryHandler(IPlanoAulaRepository planoAulaRepository)
        {
            this.planoAulaRepository = planoAulaRepository ?? throw new ArgumentNullException(nameof(planoAulaRepository));
        }

        public async Task<DateTime> Handle(ObterUltimoPlanoAulaProfessorQuery request, CancellationToken cancellationToken)
            => await planoAulaRepository.ObterUltimoPlanoAulaProfessor(request.ProfessorRf);
    }
}
