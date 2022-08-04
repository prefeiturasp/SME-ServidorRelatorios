using MediatR;
using SME.SR.Application;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP
{
    public class RelatorioBoletimEscolarUseCase : IRelatorioBoletimEscolarUseCase
    {
        private readonly IMediator mediator;

        public RelatorioBoletimEscolarUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            try
            {
                request.RotaErro = RotasRabbitSGP.RotaRelatoriosComErroBoletim;
                var relatorioQuery = request.ObterObjetoFiltro<ObterRelatorioBoletimEscolarQuery>();
                var relatorio = await mediator.Send(relatorioQuery);

                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioBoletimEscolarSimples",
                    relatorio, request.CodigoCorrelacao));
            }
            catch (Exception ex)
            {
                await mediator.Send(new SalvarLogViaRabbitCommand($"Boletim - {ex}", LogNivel.Critico));
            }
        }
    }
}

