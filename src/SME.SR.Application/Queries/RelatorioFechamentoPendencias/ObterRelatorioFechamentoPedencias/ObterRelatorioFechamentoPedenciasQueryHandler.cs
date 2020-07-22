using MediatR;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioFechamentoPedenciasQueryHandler : IRequestHandler<ObterRelatorioFechamentoPedenciasQuery, RelatorioFechamentoPendenciasDto>
    {
        private readonly IMediator mediator;

        public ObterRelatorioFechamentoPedenciasQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RelatorioFechamentoPendenciasDto> Handle(ObterRelatorioFechamentoPedenciasQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new RelatorioFechamentoPendenciasDto());
        }
    }
}
