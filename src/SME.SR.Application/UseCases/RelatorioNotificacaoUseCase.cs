using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioNotificacaoUseCase : IRelatorioNotificacaoUseCase
    {
        private readonly IMediator mediator;

        public RelatorioNotificacaoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<FiltroNotificacaoDto>();
            await mediator.Send(new GerarRelatorioNotificacaoCommand(filtros, request.CodigoCorrelacao));
       }
    }
}
