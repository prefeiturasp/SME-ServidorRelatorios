using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatorioAlteracaoNotasBimestreCommandHandler : IRequestHandler<GerarRelatorioAlteracaoNotasBimestreCommand, bool>
    {
        private readonly IMediator mediator;

        public GerarRelatorioAlteracaoNotasBimestreCommandHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(GerarRelatorioAlteracaoNotasBimestreCommand request, CancellationToken cancellationToken)
        {
            var dto = new RelatorioAlteracaoNotasBimestreDto();

            return !string.IsNullOrEmpty(await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioAlteracaoNotasBimestre", dto, request.CodigoCorrelacao, "", "Relatório de Alteração de Notas", true)));
        }
    }
}
