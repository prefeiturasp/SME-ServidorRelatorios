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
    public class RelatorioConselhoClasseAlunoUseCase: IRelatorioConselhoClasseAlunoUseCase
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
                var relatorioQuery = request.ObterObjetoFiltro<ObterRelatorioConselhoClasseAlunoQuery>();
                var relatorio = await mediator.Send(relatorioQuery);

                var relatorioSerializado = JsonConvert.SerializeObject(relatorio);

                using (SentrySdk.Init(configuration.GetSection("Sentry:DSN").Value))
                {
                    SentrySdk.CaptureMessage("5 - RelatorioConselhoClasseAlunoUseCase");
                }

                await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioConselhoClasse/ConselhoClasse", relatorioSerializado, FormatoEnum.Pdf, request.CodigoCorrelacao));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
