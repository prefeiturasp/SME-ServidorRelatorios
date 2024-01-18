using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmasPorUeCicloAnoQueryHandler : IRequestHandler<ObterTurmasPorUeCicloAnoQuery, IEnumerable<TurmaFiltradaUeCicloAnoDto>>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterTurmasPorUeCicloAnoQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<IEnumerable<TurmaFiltradaUeCicloAnoDto>> Handle(ObterTurmasPorUeCicloAnoQuery request, CancellationToken cancellationToken)
        {
            return await turmaRepository.ObterPorUeCicloAno(request.AnoLetivo, request.Ano, request.TipoCicloId, request.UeId,request.Semestre);
        }

    }
}
