using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDreUePorTurmaQueryHandler : IRequestHandler<ObterDreUePorTurmaQuery, DreUe>
    {
        private readonly ITurmaRepository turmaSgpRepository;

        public ObterDreUePorTurmaQueryHandler(ITurmaRepository turmaSgpRepository)
        {
            this.turmaSgpRepository = turmaSgpRepository ?? throw new ArgumentNullException(nameof(turmaSgpRepository));
        }

        public async Task<DreUe> Handle(ObterDreUePorTurmaQuery request, CancellationToken cancellationToken)
        {
            return await turmaSgpRepository.ObterDreUe(request.CodigoTurma);
        }
    }
}
