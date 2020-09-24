using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterCicloPorIdQueryHandler : IRequestHandler<ObterCicloPorIdQuery, TipoCiclo>
    {

        private readonly ICicloRepository cicloRepository;

        public ObterCicloPorIdQueryHandler(ICicloRepository cicloRepository)
        {
            this.cicloRepository = cicloRepository ?? throw new ArgumentNullException(nameof(cicloRepository)); ;
        }

        public async Task<TipoCiclo> Handle(ObterCicloPorIdQuery request, CancellationToken cancellationToken)
        {
            var ciclos = await cicloRepository.ObterPorId(request.CicloId);

            if (ciclos == null)
                throw new NegocioException("Não foi possível encontrar o ciclo.");

            return ciclos;
        }
    }
}
