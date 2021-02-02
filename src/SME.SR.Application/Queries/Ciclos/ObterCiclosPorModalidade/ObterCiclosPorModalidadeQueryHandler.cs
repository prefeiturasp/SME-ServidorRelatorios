using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterCiclosPorModalidadeQueryHandler : IRequestHandler<ObterCiclosPorModalidadeQuery, IEnumerable<TipoCiclo>>
    {
        private readonly ICicloRepository cicloRepository;

        public ObterCiclosPorModalidadeQueryHandler(ICicloRepository cicloRepository)
        {
            this.cicloRepository = cicloRepository ?? throw new ArgumentNullException(nameof(cicloRepository)); ;
        }

        public async Task<IEnumerable<TipoCiclo>> Handle(ObterCiclosPorModalidadeQuery request, CancellationToken cancellationToken)
        {
            var ciclos = await cicloRepository.ObterCiclosPorModalidadeAsync(request.Modalidade);

            if (ciclos == null && ciclos.Any())
                throw new NegocioException("Não foi possível encontrar os ciclos dessa modalidade.");

            return ciclos;
        }
    }
}
