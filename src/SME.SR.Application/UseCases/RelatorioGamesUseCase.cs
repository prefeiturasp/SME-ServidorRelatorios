using MediatR;
using SME.SR.Application;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP
{
    public class RelatorioGamesUseCase : IRelatorioGamesUseCase
    {
        private readonly IMediator mediator;

        public RelatorioGamesUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var gamesQuery = request.ObterObjetoFiltro<GamesQuery>();

            var nomeDoGame = await mediator.Send(gamesQuery);

           await mediator.Send(new RelatorioGamesCommand(nomeDoGame));
        }
    }
}
