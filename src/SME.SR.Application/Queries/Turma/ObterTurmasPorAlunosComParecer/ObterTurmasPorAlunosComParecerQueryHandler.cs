using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmasPorAlunosComParecerQueryHandler : IRequestHandler<ObterTurmasPorAlunosComParecerQuery, IEnumerable<AlunosTurmasCodigosDto>>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterTurmasPorAlunosComParecerQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new System.ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<IEnumerable<AlunosTurmasCodigosDto>> Handle(ObterTurmasPorAlunosComParecerQuery request, CancellationToken cancellationToken)
        {
            return await turmaRepository.ObterPorAlunosEParecerConclusivo(request.AlunosCodigos, request.PareceresConclusivosIds);
        }

    }
}
