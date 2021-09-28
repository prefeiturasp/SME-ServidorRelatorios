using MediatR;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioBoletimEscolarDetalhadoEscolaAquiUseCase : IRelatorioBoletimEscolarDetalhadoEscolaAquiUseCase
    {
        private readonly IMediator mediator;
        public RelatorioBoletimEscolarDetalhadoEscolaAquiUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task Executar(FiltroRelatorioDto request)
        {
            request.RotaErro = RotasRabbitSGP.RotaRelatoriosComErroBoletimDetalhadoEscolaAqui;
            request.RotaProcessando = RotasRabbitSR.RotaRelatoriosSolicitadosBoletimDetalhadoEscolaAqui;
            var relatorioQuery = request.ObterObjetoFiltro<ObterRelatorioBoletimEscolarDetalhadoEscolaAquiQuery>();
            var relatorio = await mediator.Send(relatorioQuery);

            await mediator.Send(new GerarRelatorioHtmlPDFBoletimDetalhadoAppCommand(relatorio, request.CodigoCorrelacao, relatorioQuery.Modalidade));
        }
    }
}
