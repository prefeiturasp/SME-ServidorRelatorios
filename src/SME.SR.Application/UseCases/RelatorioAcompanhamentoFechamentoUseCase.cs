using MediatR;
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
            var relatorioQuery = request.ObterObjetoFiltro<ObterRelatorioAcompanhamentoFechamentoQuery>();
            var relatorio = await mediator.Send(relatorioQuery);
        }
    }
}
