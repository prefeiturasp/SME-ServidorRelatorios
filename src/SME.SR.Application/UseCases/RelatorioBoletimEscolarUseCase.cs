using MediatR;
using Newtonsoft.Json;
using SME.SR.Application;
using SME.SR.Application.Queries.BoletimEscolar;
using SME.SR.Infra;
using System;
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
            //var relatorioQuery = request.ObterObjetoFiltro<ObterRelatorioBoletimEscolarQuery>();
            var relatorio = await mediator.Send((ObterRelatorioBoletimEscolarQuery)request.Filtros);

            var jsonString = JsonConvert.SerializeObject(relatorio);

            await mediator.Send(new GerarRelatorioAssincronoCommand("sme/sgp/RelatorioBoletimEscolar/BoletimEscolar", jsonString, FormatoEnum.Pdf, request.CodigoCorrelacao));
        }
    }
}
