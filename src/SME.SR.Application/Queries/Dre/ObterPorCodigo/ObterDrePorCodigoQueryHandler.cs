using MediatR;
using SME.SR.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDrePorCodigoQueryHandler : IRequestHandler<ObterDrePorCodigoQuery, Dre>
    {
        private readonly IDreRepository dreRepository;

        public ObterDrePorCodigoQueryHandler(IDreRepository dreRepository)
        {
            this.dreRepository = dreRepository ?? throw new ArgumentNullException(nameof(dreRepository));
        }
        public async Task<Dre> Handle(ObterDrePorCodigoQuery request, CancellationToken cancellationToken)
        {
            return await dreRepository.ObterPorCodigo(request.DreCodigo);
        }
    }
}
