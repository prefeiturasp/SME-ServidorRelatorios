using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterCargosServidoresPorAnoLetivoQueryHandler : IRequestHandler<ObterCargosServidoresPorAnoLetivoQuery, IEnumerable<ServidorCargoDto>>
    {
        private readonly ICargoRepository cargoRepository;

        public ObterCargosServidoresPorAnoLetivoQueryHandler(ICargoRepository cargoRepository)
        {
            this.cargoRepository = cargoRepository ?? throw new ArgumentNullException(nameof(cargoRepository)); 
        }

        public async Task<IEnumerable<ServidorCargoDto>> Handle(ObterCargosServidoresPorAnoLetivoQuery request, CancellationToken cancellationToken)
        {
            return await cargoRepository.BuscaCargosRfPorAnoLetivo(request.CodigosRF, request.AnoLetivo);
        }
    }
}
