using MediatR;
using Newtonsoft.Json;
using SME.SR.Application.Queries.RelatorioFaltasFrequencia;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioFaltasFrequenciaUseCase : IRelatorioFaltasFrequenciaUseCase
    {
        private readonly IMediator mediator;

        public RelatorioFaltasFrequenciaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var query = request.ObterObjetoFiltro<ObterRelatorioFaltasFrequenciaQuery>();
            var dadosRelatorio = await mediator.Send(query);
            var dadosJson = JsonConvert.SerializeObject(dadosRelatorio);
            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioFaltasFrequencias", dadosRelatorio, Guid.NewGuid()));
        }
    }
}
