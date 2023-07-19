using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmasPorAlunosQueryHandler : IRequestHandler<ObterTurmasPorAlunosQuery, IEnumerable<AlunosTurmasCodigosDto>>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterTurmasPorAlunosQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new System.ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<IEnumerable<AlunosTurmasCodigosDto>> Handle(ObterTurmasPorAlunosQuery request, CancellationToken cancellationToken)
        {
            return await turmaRepository.ObterPorAlunos(request.AlunosCodigos, request.AnoLetivo);
        }

    }
}
