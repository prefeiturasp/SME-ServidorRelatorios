using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterCicloPorModalidadeQueryHandler : IRequestHandler<ObterCicloPorModalidadeQuery, CicloTurmaDto>
    {
        private readonly ICicloRepository cicloRepository;

        public ObterCicloPorModalidadeQueryHandler(ICicloRepository cicloRepository)
        {
            this.cicloRepository = cicloRepository ?? throw new ArgumentNullException(nameof(cicloRepository));
        }

        public async Task<CicloTurmaDto> Handle(ObterCicloPorModalidadeQuery request, CancellationToken cancellationToken)
                => await cicloRepository.ObterCicloPorAnoModalidade(request.Ano, request.Modalidade);
    }
}
