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
            request.RotaErro = RotasRabbitSGP.RotaRelatoriosComErroBoletim;
            var relatorioQuery = request.ObterObjetoFiltro<ObterRelatorioBoletimEscolarQuery>();
            var relatorio = await mediator.Send(relatorioQuery);

            var jsonString = JsonConvert.SerializeObject(relatorio, UtilJson.ObterConfigConverterNulosEmVazio());
            
            switch (relatorioQuery.Modalidade)
            {
                case Modalidade.EJA:
                    await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioBoletimEscolarEja/BoletimEscolarEja", 
                        jsonString, TipoFormatoRelatorio.Pdf, 
                        request.CodigoCorrelacao, RotasRabbitSR.RotaRelatoriosProcessandoBoletim));
                    break;
                case Modalidade.Medio:
                    await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioBoletimEscolarMedio/BoletimEscolarMedio", 
                        jsonString, TipoFormatoRelatorio.Pdf, 
                        request.CodigoCorrelacao, RotasRabbitSR.RotaRelatoriosProcessandoBoletim));
                    break;
                default:
                    await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioBoletimEscolar/BoletimEscolar", 
                        jsonString, TipoFormatoRelatorio.Pdf, 
                        request.CodigoCorrelacao, RotasRabbitSR.RotaRelatoriosProcessandoBoletim));
                    break;
            }   
        }
    }
}
