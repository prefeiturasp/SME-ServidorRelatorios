using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sentry;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioConselhoClasseAlunoUseCase : IRelatorioConselhoClasseAlunoUseCase
    {
        private readonly IMediator mediator;
        private readonly IConfiguration configuration;

        public RelatorioConselhoClasseAlunoUseCase(IMediator mediator, IConfiguration configuration)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.configuration = configuration;
        }

        public async Task Executar(FiltroRelatorioDto request)
        {

            var relatorioQuery = request.ObterObjetoFiltro<ObterRelatorioConselhoClasseAlunoQuery>();
            SentrySdk.CaptureMessage("4.09 Obtendo relatorio.. - RelatorioConselhoClasseAlunoUseCase");

            var relatorio = await mediator.Send(relatorioQuery);

            SentrySdk.CaptureMessage("5.1 Obtive relatorio.. - RelatorioConselhoClasseAlunoUseCase");

            var urlRelatorio = "";

            if (relatorio.Relatorio.FirstOrDefault().EhBimestreFinal)
                urlRelatorio = "/sgp/RelatorioConselhoClasse/ConselhoClasseAbaFinal";
            else urlRelatorio = "/sgp/RelatorioConselhoClasse/ConselhoClasse";

            var relatorioSerializado = JsonConvert.SerializeObject(relatorio);

            SentrySdk.CaptureMessage("5.2 Serializei relatório.. - RelatorioConselhoClasseAlunoUseCase");

            SentrySdk.AddBreadcrumb("5 - Obtive o relatorio serializado : " + relatorioSerializado, "5 - RelatorioConselhoClasseAlunoUseCase");
            await mediator.Send(new GerarRelatorioAssincronoCommand(urlRelatorio, relatorioSerializado, TipoFormatoRelatorio.Pdf, request.CodigoCorrelacao, RotasRabbit.RotaRelatoriosProcessandoConselhoDeClasse));

            SentrySdk.CaptureMessage("5 FINAL - RelatorioConselhoClasseAlunoUseCase");
        }
    }
}
