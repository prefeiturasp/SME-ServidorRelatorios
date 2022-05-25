using MediatR;
using SME.SR.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public  class ObterBimestrePeriodoFechamentoAtualQueryHandler : IRequestHandler<ObterBimestrePeriodoFechamentoAtualQuery, int>
    {
        private readonly IPeriodoFechamentoRepository periodoFechamentoRepository;

        public ObterBimestrePeriodoFechamentoAtualQueryHandler(IPeriodoFechamentoRepository periodoFechamentoRepository)
        {
            this.periodoFechamentoRepository = periodoFechamentoRepository ?? throw new ArgumentNullException(nameof(periodoFechamentoRepository));
        }

        public async Task<int> Handle(ObterBimestrePeriodoFechamentoAtualQuery request, CancellationToken cancellationToken)
            => await periodoFechamentoRepository.ObterBimestrePeriodoFechamentoAtual(request.AnoLetivo);
    }
}
