using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioDeFrequenciaGlobalQueryHandler : IRequestHandler<ObterRelatorioDeFrequenciaGlobalQuery, List<FrequenciaGlobalDto>>
    {
        private readonly VariaveisAmbiente variaveisAmbiente;
        private readonly IMediator mediator;

        public ObterRelatorioDeFrequenciaGlobalQueryHandler(IMediator mediator, VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<List<FrequenciaGlobalDto>> Handle(ObterRelatorioDeFrequenciaGlobalQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
