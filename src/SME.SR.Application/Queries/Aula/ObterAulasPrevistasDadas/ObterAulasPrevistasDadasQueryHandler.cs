using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAulasPrevistasDadasQueryHandler : IRequestHandler<ObterAulasPrevistasDadasQuery, IEnumerable<AulaPrevistaBimestreQuantidade>>
    {
        private readonly IAulaPrevistaBimestreRepository aulaPrevistaBimestreRepository;

        public ObterAulasPrevistasDadasQueryHandler(IAulaPrevistaBimestreRepository aulaPrevistaBimestreRepository)
        {
            this.aulaPrevistaBimestreRepository = aulaPrevistaBimestreRepository ?? throw new ArgumentNullException(nameof(aulaPrevistaBimestreRepository));
        }
        public async Task<IEnumerable<AulaPrevistaBimestreQuantidade>> Handle(ObterAulasPrevistasDadasQuery request, CancellationToken cancellationToken)
                => await aulaPrevistaBimestreRepository.ObterBimestresAulasPrevistasPorFiltro(request.TurmaId, request.ComponenteCurricularId);
    }
}
