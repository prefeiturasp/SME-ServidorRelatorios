using MediatR;
using Newtonsoft.Json;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioAcompanhamentoFechamentoUseCase : IRelatorioAcompanhamentoFechamentoUseCase
    {
        private readonly IMediator mediator;

        public RelatorioAcompanhamentoFechamentoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            request.RotaErro = RotasRabbitSGP.RotaRelatoriosComErroAcompanhamentoFechamento;

            var relatorioQuery = request.ObterObjetoFiltro<ObterRelatorioAcompanhamentoFechamentoQuery>();
            if(relatorioQuery.UeCodigo == "-99")
            {
                var relatorioQueryConsolidado = request.ObterObjetoFiltro<ObterRelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoQuery>();
                var relatorioDto = await mediator.Send(relatorioQueryConsolidado);

                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioAcompanhamentoFechamentoConsolidadoPorUe", relatorioDto, request.CodigoCorrelacao));
            }
            else
            {
                var relatorioDto = await mediator.Send(relatorioQuery);
                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioAcompanhamentoFechamento", relatorioDto, request.CodigoCorrelacao));
            }
        }
    }
}
