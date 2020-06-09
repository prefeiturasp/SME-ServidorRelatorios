using MediatR;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterDreUePorTurmaQueryHandler : IRequestHandler<ObterDreUePorTurmaQuery, DreUe>
    {
        private ITurmaRepository _turmaSgpRepository;

        public ObterDreUePorTurmaQueryHandler(ITurmaRepository turmaSgpRepository)
        {
            this._turmaSgpRepository = turmaSgpRepository;
        }

        public async Task<DreUe> Handle(ObterDreUePorTurmaQuery request, CancellationToken cancellationToken)
        {
            return await _turmaSgpRepository.ObterDreUe(request.CodigoTurma);
        }
    }
}
