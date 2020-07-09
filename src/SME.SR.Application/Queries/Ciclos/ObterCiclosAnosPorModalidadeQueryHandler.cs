using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterCiclosAnosPorModalidadeQueryHandler : IRequestHandler<ObterCiclosAnosPorModalidadeQuery, IEnumerable<TipoCiclo>>
    {
        private readonly ICicloRepository cicloRepository;

        public ObterCiclosAnosPorModalidadeQueryHandler(ICicloRepository cicloRepository)
        {
            this.cicloRepository = cicloRepository ?? throw new ArgumentNullException(nameof(cicloRepository)); ;
        }

        public async Task<IEnumerable<TipoCiclo>> Handle(ObterCiclosAnosPorModalidadeQuery request, CancellationToken cancellationToken)
        {
            var ciclos = await cicloRepository.ObterCiclosPorAnosModalidade(request.Anos, request.Modalidade);

            if (ciclos == null && ciclos.Any())
                throw new NegocioException("Não foi possível encontrar os ciclos dos anos e na modalidade.");

            return ciclos;
        }
    }
}
