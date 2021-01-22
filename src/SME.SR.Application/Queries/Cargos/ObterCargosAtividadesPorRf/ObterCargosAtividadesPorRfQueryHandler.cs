using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterCargosAtividadesPorRfQueryHandler : IRequestHandler<ObterCargosAtividadesPorRfQuery, IEnumerable<ServidorCargoDto>>
    {
        private readonly ICargoRepository cargoRepository;

        public ObterCargosAtividadesPorRfQueryHandler(ICargoRepository cargoRepository)
        {
            this.cargoRepository = cargoRepository ?? throw new ArgumentNullException(nameof(cargoRepository));
        }

        public async Task<IEnumerable<ServidorCargoDto>> Handle(ObterCargosAtividadesPorRfQuery request, CancellationToken cancellationToken)
        {
            return await cargoRepository.BuscarCargosAtividades(request.CodigosRf);
        }
    }
}
