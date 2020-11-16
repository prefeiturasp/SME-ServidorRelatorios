using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterUePorCodigosQueryHandler : IRequestHandler<ObterUePorCodigosQuery, IEnumerable<Ue>>
    {
        private readonly IUeRepository ueRepository;

        public ObterUePorCodigosQueryHandler(IUeRepository ueRepository)
        {
            this.ueRepository = ueRepository ?? throw new ArgumentNullException(nameof(UeRepository));
        }

        public async Task<IEnumerable<Ue>> Handle(ObterUePorCodigosQuery request, CancellationToken cancellationToken)
        {
            var ues = await ueRepository.ObterPorCodigos(request.UeCodigos);

            if (ues == null || ues.Any())
            {
                throw new NegocioException("Não foi possível localizar as ues");
            }

            return ues;
        }
    }
}
