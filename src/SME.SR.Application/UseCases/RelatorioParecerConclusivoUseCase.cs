using MediatR;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioParecerConclusivoUseCase : IRelatorioParecerConclusivoUseCase
    {
        private readonly IMediator mediator;

        public RelatorioParecerConclusivoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<FiltroRelatorioParecerConclusivoDto>();

            var resultado = await mediator.Send(new ObterRelatorioParecerConclusivoQuery() { filtroRelatorioParecerConclusivoDto = filtros });

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioParecerConclusivo", resultado, request.CodigoCorrelacao));

        }

    }
}
