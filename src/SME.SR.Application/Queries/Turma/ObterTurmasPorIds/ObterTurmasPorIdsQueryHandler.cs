using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmasPorIdsQueryHandler : IRequestHandler<ObterTurmasPorIdsQuery, IEnumerable<Turma>>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterTurmasPorIdsQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<IEnumerable<Turma>> Handle(ObterTurmasPorIdsQuery request, CancellationToken cancellationToken)
            => await turmaRepository.ObterTurmasPorIds(request.Ids);
    }
}
