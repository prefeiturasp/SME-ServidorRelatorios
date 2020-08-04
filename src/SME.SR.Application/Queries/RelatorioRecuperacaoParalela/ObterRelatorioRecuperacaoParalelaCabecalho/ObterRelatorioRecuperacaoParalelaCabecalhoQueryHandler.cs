using MediatR;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioRecuperacaoParalelaCabecalhoQueryHandler : IRequestHandler<ObterRelatorioRecuperacaoParalelaCabecalhoQuery, RelatorioRecuperacaoParalelaDto>
    {
        private readonly IMediator mediator;

        public ObterRelatorioRecuperacaoParalelaCabecalhoQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RelatorioRecuperacaoParalelaDto> Handle(ObterRelatorioRecuperacaoParalelaCabecalhoQuery request, CancellationToken cancellationToken)
        {
            var filtros = request.TurmaCodigo;
            return null;
        }
    }
}
