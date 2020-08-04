using MediatR;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioRecuperacaoParalelaQueryHandler : IRequestHandler<ObterRelatorioRecuperacaoParalelaQuery, RelatorioRecuperacaoParalelaDto>
    {
        private readonly IMediator mediator;

        public ObterRelatorioRecuperacaoParalelaQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RelatorioRecuperacaoParalelaDto> Handle(ObterRelatorioRecuperacaoParalelaQuery request, CancellationToken cancellationToken)
        {
            var filtros = request.AlunoCodigo;
            return null;
        }
    }
}
