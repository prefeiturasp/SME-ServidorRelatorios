using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmasPorCodigoQueryHandler : IRequestHandler<ObterTurmasPorCodigoQuery, IEnumerable<Turma>>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterTurmasPorCodigoQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<IEnumerable<Turma>> Handle(ObterTurmasPorCodigoQuery request, CancellationToken cancellationToken)
            => await turmaRepository.ObterTurmasPorCodigos(request.Codigos);
    }
}
