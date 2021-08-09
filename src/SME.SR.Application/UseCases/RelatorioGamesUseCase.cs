using MediatR;
using Newtonsoft.Json;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
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
            request.RotaErro = RotasRabbitSGP.RotaRelatoriosComErroConselhoDeClasse;
            var gamesQuery = request.ObterObjetoFiltro<GamesQuery>();
            var nomeDoGame = await mediator.Send(gamesQuery);

            var conselhoClasse = await mediator.Send(new RelatorioExemploQuery());
            var dadosRelatorio = JsonConvert.SerializeObject(conselhoClasse);

            await mediator.Send(new GerarRelatorioAssincronoCommand("/sme/sgp/RelatorioConselhoClasse/ConselhoClasse",
                                                                    dadosRelatorio,
                                                                    TipoFormatoRelatorio.Pdf,
                                                                    request.CodigoCorrelacao, RotasRabbitSR.RotaRelatoriosProcessandoConselhoDeClasse));
        }
    }
}
