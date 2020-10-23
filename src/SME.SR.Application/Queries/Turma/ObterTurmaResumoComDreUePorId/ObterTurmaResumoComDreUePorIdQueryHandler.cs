using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmaResumoComDreUePorIdQueryHandler : IRequestHandler<ObterTurmaResumoComDreUePorIdQuery, TurmaResumoDto>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterTurmaResumoComDreUePorIdQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<TurmaResumoDto> Handle(ObterTurmaResumoComDreUePorIdQuery request, CancellationToken cancellationToken)
            => await turmaRepository.ObterTurmaResumoComDreUePorId(request.TurmaId);
    }
}
