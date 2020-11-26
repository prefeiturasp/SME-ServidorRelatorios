using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatorioAlteracaoNotasCommandHandler : IRequestHandler<GerarRelatorioAlteracaoNotasCommand, bool>
    {
        private readonly IMediator mediator;

        public GerarRelatorioAlteracaoNotasCommandHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(GerarRelatorioAlteracaoNotasCommand request, CancellationToken cancellationToken)
        {
            var dto = new RelatorioAlteracaoNotasDto();

            return !string.IsNullOrEmpty(await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioAlteracaoNotasBimestre", dto, request.CodigoCorrelacao, "", "Relatório de Alteração de Notas", true)));
        }
    }
}
