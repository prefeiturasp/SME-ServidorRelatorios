using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterPlanoAnualQueryHandler : IRequestHandler<ObterPlanoAnualQuery, PlanoAnualDto>
    {
        private readonly IPlanoAnualRepository planoAnualRepository;
        
        public ObterPlanoAnualQueryHandler(IPlanoAnualRepository planoAnualRepository)
        {
            this.planoAnualRepository = planoAnualRepository ?? throw new ArgumentNullException(nameof(planoAnualRepository));
        }
        public async Task<PlanoAnualDto> Handle(ObterPlanoAnualQuery request, CancellationToken cancellationToken)
        {
            var planoAnual = await planoAnualRepository.ObterPorId(request.Id);

            if (planoAnual == null)
                throw new NegocioException("Plano Anual não encontrado.");

            return planoAnual;
        }
    }
}
