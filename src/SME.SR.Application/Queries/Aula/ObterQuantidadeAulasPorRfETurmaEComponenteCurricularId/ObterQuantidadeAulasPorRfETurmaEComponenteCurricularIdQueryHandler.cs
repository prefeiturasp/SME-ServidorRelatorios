using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    class ObterQuantidadeAulasPorRfETurmaEComponenteCurricularIdQueryHandler : IRequestHandler<ObterQuantidadeAulasPorRfETurmaEComponenteCurricularIdQuery, int>
    {
        private readonly IAulaRepository aulaRepository;

        public ObterQuantidadeAulasPorRfETurmaEComponenteCurricularIdQueryHandler(IAulaRepository aulaRepository)
        {
            this.aulaRepository = aulaRepository ?? throw new ArgumentNullException(nameof(aulaRepository));
        }

        public async Task<int> Handle(ObterQuantidadeAulasPorRfETurmaEComponenteCurricularIdQuery request, CancellationToken cancellationToken) 
            => await aulaRepository.ObterQuantidadeAulas(request.TurmaId,
                                                    request.ComponenteCurricularId.ToString(),
                                                    request.CodigoRF);
    }
}
