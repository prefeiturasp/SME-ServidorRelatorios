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
            request.RotaErro = RotasRabbitSGP.RotaRelatoriosComErroConselhoDeClasse;
            var relatorioQuery = request.ObterObjetoFiltro<ObterRelatorioConselhoClasseAlunoQuery>();

            var relatorio = await mediator.Send(relatorioQuery);

            string urlRelatorio;
            if (relatorio.Relatorio.FirstOrDefault().EhBimestreFinal)
                urlRelatorio = "/sgp/RelatorioConselhoClasse/ConselhoClasseAbaFinal";
            else urlRelatorio = "/sgp/RelatorioConselhoClasse/ConselhoClasse";

            var relatorioSerializado = JsonConvert.SerializeObject(relatorio);

            await mediator.Send(new GerarRelatorioAssincronoCommand(urlRelatorio, relatorioSerializado, TipoFormatoRelatorio.Pdf, request.CodigoCorrelacao, RotasRabbitSR.RotaRelatoriosProcessandoConselhoDeClasse));
        }
    }
}
