using MediatR;
using Newtonsoft.Json;
using SME.SR.Application;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;
using static SME.SR.Infra.Enumeradores;

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
            var relatorioQuery = request.ObterObjetoFiltro<ObterRelatorioBoletimEscolarQuery>();
            var relatorio = await mediator.Send(relatorioQuery);

            var jsonString = JsonConvert.SerializeObject(relatorio, UtilJson.ObterConfigConverterNulosEmVazio());

            if (relatorioQuery.Modalidade == Modalidade.EJA)
                await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioBoletimEscolarEja/BoletimEscolarEja", jsonString, TipoFormatoRelatorio.Pdf, request.CodigoCorrelacao));
            else
                await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioBoletimEscolar/BoletimEscolar", jsonString, TipoFormatoRelatorio.Pdf, request.CodigoCorrelacao));
        }
    }
}
