using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDrePorIdQueryHandler : IRequestHandler<ObterDrePorIdQuery, Dre>
    {
        private readonly IDreRepository dreRepository;

        public ObterDrePorIdQueryHandler(IDreRepository dreRepository)
        {
            this.dreRepository = dreRepository ?? throw new ArgumentNullException(nameof(dreRepository));
        }
        public async Task<Dre> Handle(ObterDrePorIdQuery request, CancellationToken cancellationToken)
        {
            var dre = await dreRepository.ObterPorId(request.DreId);

            if (dre == null)
            {
                throw new NegocioException("Não foi possível localizar a Dre");
            }

            return dre;
        }
    }
}
