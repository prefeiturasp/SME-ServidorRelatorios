using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    class ObterAulasDadasNoBimestreQueryHandler : IRequestHandler<ObterAulasDadasNoBimestreQuery, int>
    {
        private readonly IAulaRepository aulaRepository;

        public ObterAulasDadasNoBimestreQueryHandler(IAulaRepository aulaRepository)
        {
            this.aulaRepository = aulaRepository ?? throw new ArgumentNullException(nameof(aulaRepository));
        }

        public async Task<int> Handle(ObterAulasDadasNoBimestreQuery request, CancellationToken cancellationToken)
            => await aulaRepository.ObterAulasDadas(request.TurmaCodigo,
                                                    request.ComponentesCurricularesCodigo,
                                                    request.TipoCalendarioId,
                                                    request.Bimestre);
    }
}
