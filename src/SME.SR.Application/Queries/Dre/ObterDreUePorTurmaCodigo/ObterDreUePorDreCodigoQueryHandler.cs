using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDreUePorDreCodigoQueryHandler : IRequestHandler<ObterDreUePorDreCodigoQuery, DreUe>
    {
        private readonly IDreRepository dreRepository;

        public ObterDreUePorDreCodigoQueryHandler(IDreRepository dreRepository)
        {
            this.dreRepository = dreRepository ?? throw new System.ArgumentNullException(nameof(dreRepository));
        }

        public async Task<DreUe> Handle(ObterDreUePorDreCodigoQuery request, CancellationToken cancellationToken)
            => await dreRepository.ObterDreUePorDreUeCodigo(request.DreCodigo,request.UeCodigo);
    }
}
