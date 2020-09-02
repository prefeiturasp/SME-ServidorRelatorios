using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public  class ObterTurmasPorAlunosSemParecerQueryHandler : IRequestHandler<ObterTurmasPorAlunosSemParecerQuery, IEnumerable<AlunosTurmasCodigosDto>>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterTurmasPorAlunosSemParecerQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new System.ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<IEnumerable<AlunosTurmasCodigosDto>> Handle(ObterTurmasPorAlunosSemParecerQuery request, CancellationToken cancellationToken)
        {
            return await turmaRepository.ObterPorAlunosSemParecerConclusivo(request.AlunosCodigos);
        }

    }
}
