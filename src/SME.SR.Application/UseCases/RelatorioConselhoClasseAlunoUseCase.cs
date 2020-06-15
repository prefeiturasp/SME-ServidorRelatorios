using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sentry;
using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;
using static SME.SR.Infra.Enumeradores;

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

            using (SentrySdk.Init(configuration.GetSection("Sentry:DSN").Value))
            {
                try
                {

                    var relatorioQuery = request.ObterObjetoFiltro<ObterRelatorioConselhoClasseAlunoQuery>();


                    SentrySdk.CaptureMessage("4.09 Obtendo relatorio.. - RelatorioConselhoClasseAlunoUseCase");
                    //SentrySdk.AddBreadcrumb("Obtendo relatório..", "5 - RelatorioConselhoClasseAlunoUseCase");

                    //RelatorioConselhoClasseArray relatorio = await mediator.Send(relatorioQuery);

                    SentrySdk.CaptureMessage("5.1 Obtive relatorio.. - RelatorioConselhoClasseAlunoUseCase");
                    //SentrySdk.AddBreadcrumb("Obtive o relatório", "5 - RelatorioConselhoClasseAlunoUseCase");

                    //var relatorioSerializado = JsonConvert.SerializeObject(relatorio);

                    SentrySdk.CaptureMessage("5.2 Serializei relatório.. - RelatorioConselhoClasseAlunoUseCase");

                    //SentrySdk.AddBreadcrumb("5 - Obtive o relatorio serializado : " + relatorioSerializado, "5 - RelatorioConselhoClasseAlunoUseCase");

                    //await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioConselhoClasse/ConselhoClasse", relatorioSerializado, FormatoEnum.Pdf, request.CodigoCorrelacao));

                    //SentrySdk.CaptureMessage("5 FINAL - RelatorioConselhoClasseAlunoUseCase");


                }
                catch (Exception ex)
                {
                    SentrySdk.CaptureException(ex);
                    throw ex;
                }
            }

        }
    }




}
