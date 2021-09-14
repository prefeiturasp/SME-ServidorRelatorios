using MediatR;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoUseCase : IRelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoUseCase
    {
        private readonly IMediator mediator;

        public RelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task Executar(FiltroRelatorioDto request)
        {
            request.RotaErro = RotasRabbitSGP.RotaRelatoriosComErroAcompanhamentoFechamento;
            var relatorioQuery = request.ObterObjetoFiltro<ObterRelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoQuery>();
            var relatorioDto = await mediator.Send(relatorioQuery);

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioAcompanhamentoFechamento", relatorioDto, request.CodigoCorrelacao));
        }
    }
}
