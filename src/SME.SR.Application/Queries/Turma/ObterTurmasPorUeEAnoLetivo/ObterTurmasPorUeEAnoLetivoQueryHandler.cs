using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmasPorUeEAnoLetivoQueryHandler : IRequestHandler<ObterTurmasPorUeEAnoLetivoQuery, IEnumerable<Turma>>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterTurmasPorUeEAnoLetivoQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository;
        }

        public async Task<IEnumerable<Turma>> Handle(ObterTurmasPorUeEAnoLetivoQuery request, CancellationToken cancellationToken)
                => await turmaRepository.ObterTurmasPorUeEAnoLetivo(request.CodigoUE, request.AnoLetivo);
    }
}
