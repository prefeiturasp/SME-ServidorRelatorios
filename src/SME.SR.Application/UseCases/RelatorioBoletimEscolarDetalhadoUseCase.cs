using MediatR;
using Newtonsoft.Json;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioBoletimEscolarDetalhadoUseCase : IRelatorioBoletimEscolarDetalhadoUseCase
    {
        private readonly IMediator mediator;

        public RelatorioBoletimEscolarDetalhadoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var relatorioQuery = request.ObterObjetoFiltro<ObterRelatorioBoletimEscolarDetalhadoQuery>();
            var relatorio = await mediator.Send(relatorioQuery);

            switch (relatorioQuery.Modalidade)
            {
                case Modalidade.EJA:
                    await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("BoletimEscolarEja", relatorio, request.CodigoCorrelacao));
                    break;
                case Modalidade.Medio:
                    await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("BoletimEscolarMedio", relatorio, request.CodigoCorrelacao));
                    break;
                default:
                    await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("BoletimEscolar", relatorio, request.CodigoCorrelacao));
                    break;
            }
        }
    }
}
