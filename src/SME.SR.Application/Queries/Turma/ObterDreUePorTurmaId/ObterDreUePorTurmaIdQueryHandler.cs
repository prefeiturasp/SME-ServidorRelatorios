using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDreUePorTurmaIdQueryHandler : IRequestHandler<ObterDreUePorTurmaIdQuery, DreUe>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterDreUePorTurmaIdQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new System.ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<DreUe> Handle(ObterDreUePorTurmaIdQuery request, CancellationToken cancellationToken)
            => await turmaRepository.ObterDreUePorTurmaId(request.TurmaId);
    }
}
