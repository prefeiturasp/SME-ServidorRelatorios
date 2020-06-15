using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sentry;
using SME.SR.Application.Interfaces;
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

            try
            {

                using (SentrySdk.Init(configuration.GetSection("Sentry:DSN").Value))
                {
                    var relatorioQuery = request.ObterObjetoFiltro<ObterRelatorioConselhoClasseAlunoQuery>();

                    SentrySdk.AddBreadcrumb("Obtendo relatório..", "5 - RelatorioConselhoClasseAlunoUseCase");

                    var relatorio = await mediator.Send(relatorioQuery);
                    
                    var relatorioSerializado = JsonConvert.SerializeObject(relatorio);


                    SentrySdk.AddBreadcrumb("5 - Obtive o relatorio serializado : " + relatorioSerializado, "5 - RelatorioConselhoClasseAlunoUseCase");

                    await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioConselhoClasse/ConselhoClasse", relatorioSerializado, FormatoEnum.Pdf, request.CodigoCorrelacao));

                    SentrySdk.CaptureMessage("5 - RelatorioConselhoClasseAlunoUseCase");

                }

            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }

        }
    }




}
