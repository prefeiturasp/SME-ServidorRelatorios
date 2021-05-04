using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterComDreUePorTurmaIdQueryHandler : IRequestHandler<ObterComDreUePorTurmaIdQuery, Turma>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterComDreUePorTurmaIdQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<Turma> Handle(ObterComDreUePorTurmaIdQuery request, CancellationToken cancellationToken)
        {
            var turma = await turmaRepository.ObterComDreUePorId(request.TurmaId);
            return turma;
        }
    }
}
