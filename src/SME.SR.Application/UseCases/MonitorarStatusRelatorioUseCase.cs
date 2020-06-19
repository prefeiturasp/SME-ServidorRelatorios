using MediatR;
using Microsoft.Extensions.Configuration;
using Sentry;
using SME.SR.Application.Interfaces;
using SME.SR.Application.Queries.Comum.Relatorios;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class MonitorarStatusRelatorioUseCase : IMonitorarStatusRelatorioUseCase
    {
        private readonly IMediator mediator;
        private readonly IServicoFila servicoFila;
        private readonly IConfiguration configuration;

        public MonitorarStatusRelatorioUseCase(IMediator mediator, IServicoFila servicoFila, IConfiguration configuration)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
            this.servicoFila = servicoFila ?? throw new System.ArgumentNullException(nameof(servicoFila));
            this.configuration = configuration ?? throw new System.ArgumentNullException(nameof(configuration));
        }

        public async Task Executar(FiltroRelatorioDto filtroRelatorioDto)
        {
            using (SentrySdk.Init(configuration.GetSection("Sentry:DSN").Value))
            {
                SentrySdk.AddBreadcrumb($"Obtendo dados do relatório", "8 - MonitorarStatusRelatorioUseCase");

                var dadosRelatorio = filtroRelatorioDto.ObterObjetoFiltro<DadosRelatorioDto>();

                var detalhesRelatorio = await mediator.Send(new ObterDetalhesRelatorioQuery(dadosRelatorio.RequisicaoId, dadosRelatorio.JSessionId));

                SentrySdk.AddBreadcrumb($"dados do relatório OK", "8 - MonitorarStatusRelatorioUseCase");


                if (detalhesRelatorio != null && detalhesRelatorio.Pronto)
                {
                    SentrySdk.AddBreadcrumb($"Indo publicar na fila Prontos..", "8 - MonitorarStatusRelatorioUseCase");
                    servicoFila.PublicaFila(new PublicaFilaDto(dadosRelatorio, RotasRabbit.FilaClientsSgp, RotasRabbit.RotaRelatoriosProntosSgp, null, filtroRelatorioDto.CodigoCorrelacao));
                    SentrySdk.CaptureMessage("8 - MonitorarStatusRelatorioUseCase - Publicado na fila PRONTO OK!");
                }
                else
                {
                    SentrySdk.AddBreadcrumb($"Indo publicar na fila Processando..", "8 - MonitorarStatusRelatorioUseCase");                    
                    UtilTimer.SetTimeout(5000, () => servicoFila.PublicaFila(new PublicaFilaDto(dadosRelatorio, RotasRabbit.FilaWorkerRelatorios, RotasRabbit.RotaRelatoriosProcessando, null, filtroRelatorioDto.CodigoCorrelacao)));
                    SentrySdk.CaptureMessage("8 - MonitorarStatusRelatorioUseCase - Publicado na fila Processando -> Não está pronto ou Erro!");
                }
            }
        }
    }
}
