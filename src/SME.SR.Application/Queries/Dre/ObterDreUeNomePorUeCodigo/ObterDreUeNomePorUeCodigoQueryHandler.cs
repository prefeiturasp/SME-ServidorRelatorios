using MediatR;
using SME.SR.Data;
using SME.SR.Data.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.Dre.ObterDreUeNomePorUeCodigo
{
    public class ObterDreUeNomePorUeCodigoQueryHandler : IRequestHandler<ObterDreUeNomePorUeCodigoQuery, DreUeNome>
    {
        private readonly IDreRepository dreRepository;

        public ObterDreUeNomePorUeCodigoQueryHandler(IDreRepository dreRepository)
        {
            this.dreRepository = dreRepository ?? throw new ArgumentNullException(nameof(dreRepository));
        }

        public async Task<DreUeNome> Handle(ObterDreUeNomePorUeCodigoQuery request, CancellationToken cancellationToken)
        {
            return await dreRepository.ObterNomeDreUePorUeCodigo(request.UeCodigo);
        }
    }
}
