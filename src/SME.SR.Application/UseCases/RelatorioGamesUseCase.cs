using MediatR;
using Newtonsoft.Json;
using SME.SR.Application;
using SME.SR.Application.Queries.Exemplos.RelatorioExemplo;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;
using static SME.SR.Infra.Enumeradores.Enumeradores;

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
            
            var conselhoClasse = await mediator.Send(new RelatorioExemploQuery());
            var dadosRelatorio = JsonConvert.SerializeObject(conselhoClasse);

            await mediator.Send(new GerarRelatorioAssincronoCommand("sme/sgp/RelatorioConselhoClasse/ConselhoClasse", dadosRelatorio, FormatoEnum.Pdf));
        }
    }
}
