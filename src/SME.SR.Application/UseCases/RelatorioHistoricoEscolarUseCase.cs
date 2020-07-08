using MediatR;
using Newtonsoft.Json;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioHistoricoEscolarUseCase : IRelatorioHistoricoEscolarUseCase
    {
        private readonly IMediator mediator;

        public RelatorioHistoricoEscolarUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var relatorioQuery = request.ObterObjetoFiltro<FiltroHistoricoEscolarDto>();
            var relatorioHistoricoEscolar = await mediator.Send(relatorioQuery);
            var jsonString = "";
            if (relatorioHistoricoEscolar != null)
            {
                jsonString = JsonConvert.SerializeObject(relatorioHistoricoEscolar);
            }

            await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioHistoricoEscolarFundamental", jsonString, TipoFormatoRelatorio.Pdf, request.CodigoCorrelacao));
        }
    }
}
