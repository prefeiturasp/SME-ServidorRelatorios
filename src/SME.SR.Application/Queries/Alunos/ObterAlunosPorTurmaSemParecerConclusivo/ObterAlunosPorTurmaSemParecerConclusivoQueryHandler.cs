using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosPorTurmaSemParecerConclusivoQueryHandler : IRequestHandler<ObterAlunosPorTurmaSemParecerConclusivoQuery, IEnumerable<AlunosTurmasCodigosDto>>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterAlunosPorTurmaSemParecerConclusivoQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<IEnumerable<AlunosTurmasCodigosDto>> Handle(ObterAlunosPorTurmaSemParecerConclusivoQuery request, CancellationToken cancellationToken)
        {
            return await turmaRepository.ObterAlunosCodigosPorTurmaSemParecerConclusivo(request.TurmaCodigo);
        }
    }
}
