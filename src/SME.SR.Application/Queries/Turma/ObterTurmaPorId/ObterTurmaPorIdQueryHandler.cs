using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmaPorIdQueryHandler : IRequestHandler<ObterTurmaPorIdQuery, Turma>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterTurmaPorIdQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<Turma> Handle(ObterTurmaPorIdQuery request, CancellationToken cancellationToken)
            => await turmaRepository.ObterPorId(request.Id);
    }
}
