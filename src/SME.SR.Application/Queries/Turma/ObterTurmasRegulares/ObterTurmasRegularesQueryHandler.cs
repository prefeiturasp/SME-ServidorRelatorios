using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmasRegularesQueryHandler : IRequestHandler<ObterTurmasRegularesQuery, IEnumerable<string>>
    {
        private readonly ITurmaRepository turmaRepository;
        public ObterTurmasRegularesQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }

        public Task<IEnumerable<string>> Handle(ObterTurmasRegularesQuery request, CancellationToken cancellationToken)
        {
            return this.turmaRepository.ObterCodigoTurmaRegulares(request.CodigosAlunos);
        }
    }
}
