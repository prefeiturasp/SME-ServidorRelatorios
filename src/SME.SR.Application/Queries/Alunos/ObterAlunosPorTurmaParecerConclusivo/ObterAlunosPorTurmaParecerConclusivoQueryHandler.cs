using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosPorTurmaParecerConclusivoQueryHandler : IRequestHandler<ObterAlunosPorTurmaParecerConclusivoQuery, IEnumerable<AlunosTurmasCodigosDto>>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterAlunosPorTurmaParecerConclusivoQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<IEnumerable<AlunosTurmasCodigosDto>> Handle(ObterAlunosPorTurmaParecerConclusivoQuery request, CancellationToken cancellationToken)
        {
            return await turmaRepository.ObterAlunosCodigosPorTurmaParecerConclusivo(request.TurmaCodigo, request.PareceresConclusivosIds);
        }
    }
}
