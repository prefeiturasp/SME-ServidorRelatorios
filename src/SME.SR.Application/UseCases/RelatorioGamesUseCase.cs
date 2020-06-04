using MediatR;
using SME.SR.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP
{
    public class RelatorioGamesUseCase : IRelatorioGamesUseCase
    {
        private readonly IMediator mediator;

        public RelatorioGamesUseCase(IMediator mediator, int ano)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task Executar(int ano)
        {

            var nomeDoGame = await mediator.Send(new GamesQuery(ano));

            var envioDoRelatorio = await mediator.Send(new RelatorioGamesCommand(nomeDoGame));
        }
    }
}
