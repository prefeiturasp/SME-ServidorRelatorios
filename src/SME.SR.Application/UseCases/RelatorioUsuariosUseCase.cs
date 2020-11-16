using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioUsuariosUseCase : IRelatorioUsuariosUseCase
    {
        private readonly IMediator mediator;

        public RelatorioUsuariosUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {

            //await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioUsuarios", dtoRelatorio, request.CodigoCorrelacao));
        }
    }
}
