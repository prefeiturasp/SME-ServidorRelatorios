using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDreUePorTurmaCodigoQueryHandler : IRequestHandler<ObterDreUePorTurmaCodigoQuery, DreUe>
    {
        private readonly IDreRepository dreRepository;

        public ObterDreUePorTurmaCodigoQueryHandler(IDreRepository dreRepository)
        {
            this.dreRepository = dreRepository ?? throw new System.ArgumentNullException(nameof(dreRepository));
        }

        public async Task<DreUe> Handle(ObterDreUePorTurmaCodigoQuery request, CancellationToken cancellationToken)
            => await dreRepository.ObterDreUePorTurmaCodigo(request.TurmaCodigo);
    }
}
