using MediatR;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;
using static SME.SR.Infra.Enumeradores;

namespace SME.SR.Application
{
    public class RelatorioHistoricoEscolarUseCase : IRelatorioHistoricoEscolarUseCase
    {
        private readonly IMediator mediator;

        public RelatorioHistoricoEscolarUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var relatorioQuery = request.ObterObjetoFiltro<ObterHistoricoEscolarQueryHandler>();
            var relatorioHistoricoEscolar = await mediator.Send(relatorioQuery);

            string jsonString = "";
            var urlRelatorio = "";
            await mediator.Send(new GerarRelatorioAssincronoCommand(urlRelatorio, jsonString, FormatoEnum.Pdf, request.CodigoCorrelacao));
        }
    }
}
