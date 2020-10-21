using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAulasPrevistasDadasQueryHandler : IRequestHandler<ObterAulasPrevistasDadasQuery, AulaPrevista>
    {
        private readonly IAulaRepository aulaRepository;

        public ObterAulasPrevistasDadasQueryHandler(IAulaRepository aulaRepository)
        {
            this.aulaRepository = aulaRepository ?? throw new ArgumentNullException(nameof(aulaRepository));
        }
        public Task<AulaPrevista> Handle(ObterAulasPrevistasDadasQuery request, CancellationToken cancellationToken)
                => aulaRepository.ObterAulaPrevistaFiltro(request.TipoCalendarioId, request.TurmaId, request.DisciplinaId);
    }
}
