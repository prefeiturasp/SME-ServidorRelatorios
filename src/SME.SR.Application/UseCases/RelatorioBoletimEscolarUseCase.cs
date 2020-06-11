using MediatR;
using Newtonsoft.Json;
using SME.SR.Application;
using SME.SR.Application.Queries.RelatorioBoletimEscolar;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;
using static SME.SR.Infra.Enumeradores.Enumeradores;

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
            var gamesQuery = request.ObterObjetoFiltro<RelatorioBoletimEscolarQuery>();
            var nomeDoGame = await mediator.Send(gamesQuery);
            
            var conselhoClasse = await mediator.Send(new RelatorioBoletimEscolarQuery());
            var dadosRelatorio = JsonConvert.SerializeObject(conselhoClasse);

            await mediator.Send(new GerarRelatorioAssincronoCommand("sme/sgp/RelatorioBoletimEscolar/BoletimEscolar", dadosRelatorio, FormatoEnum.Pdf));
        }
    }
}
