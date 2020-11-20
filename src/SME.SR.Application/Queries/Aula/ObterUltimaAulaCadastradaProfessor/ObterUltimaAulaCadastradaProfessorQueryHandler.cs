using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterUltimaAulaCadastradaProfessorQueryHandler : IRequestHandler<ObterUltimaAulaCadastradaProfessorQuery, DateTime?>
    {
        private readonly IAulaRepository aulaRepository;

        public ObterUltimaAulaCadastradaProfessorQueryHandler(IAulaRepository aulaRepository)
        {
            this.aulaRepository = aulaRepository ?? throw new ArgumentNullException(nameof(aulaRepository));
        }

        public async Task<DateTime?> Handle(ObterUltimaAulaCadastradaProfessorQuery request, CancellationToken cancellationToken)
            => await aulaRepository.ObterUltimaAulaCadastradaProfessor(request.ProfessorRf);
    }
}
