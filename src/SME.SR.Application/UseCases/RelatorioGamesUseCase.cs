using MediatR;
using Newtonsoft.Json.Linq;
using SME.SR.Application;
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

        public async Task Executar(JObject request)
        {
            var ano = int.Parse(request["Dados"]["ano"].ToString());

            var nomeDoGame = await mediator.Send(new GamesQuery(ano));

            var envioDoRelatorio = await mediator.Send(new RelatorioGamesCommand(nomeDoGame));
        }
    }
}
