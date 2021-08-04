using MediatR;
using Microsoft.Extensions.Configuration;
using Sentry;
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

        public MonitorarStatusRelatorioUseCase(IMediator mediator, IServicoFila servicoFila)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
            this.servicoFila = servicoFila ?? throw new System.ArgumentNullException(nameof(servicoFila));            
        }

        public async Task Executar(FiltroRelatorioDto filtroRelatorioDto)
        {
            SentrySdk.AddBreadcrumb($"Obtendo dados do relatório", "8 - MonitorarStatusRelatorioUseCase");

            var dadosRelatorio = filtroRelatorioDto.ObterObjetoFiltro<DadosRelatorioDto>();

            var detalhesRelatorio = await mediator.Send(new ObterDetalhesRelatorioQuery(dadosRelatorio.RequisicaoId, dadosRelatorio.JSessionId));

            SentrySdk.AddBreadcrumb($"dados do relatório OK", "8 - MonitorarStatusRelatorioUseCase");


            if (detalhesRelatorio != null && detalhesRelatorio.Pronto)
            {
                SentrySdk.AddBreadcrumb($"Indo publicar na fila Prontos..", "8 - MonitorarStatusRelatorioUseCase");

                //TODO: Aplicar Polly ??
                if (await mediator.Send(new SalvarRelatorioJasperLocalCommand(dadosRelatorio.JSessionId, dadosRelatorio.RequisicaoId, dadosRelatorio.ExportacaoId, dadosRelatorio.CodigoCorrelacao)))
                {
                    servicoFila.PublicaFila(new PublicaFilaDto(dadosRelatorio, RotasRabbit.RotaRelatoriosProntosSgp, ExchangeRabbit.ExchangeSgp, filtroRelatorioDto.CodigoCorrelacao));
                    SentrySdk.CaptureMessage("8 - MonitorarStatusRelatorioUseCase - Publicado na fila PRONTO OK!");
                }
                else PublicarNovamenteNaFila(filtroRelatorioDto, dadosRelatorio);

            }
            else
            {
                PublicarNovamenteNaFila(filtroRelatorioDto, dadosRelatorio);
            }
        }

        private void PublicarNovamenteNaFila(FiltroRelatorioDto filtroRelatorioDto, DadosRelatorioDto dadosRelatorio)
        {
            SentrySdk.AddBreadcrumb($"Indo publicar na fila Processando..", "8 - MonitorarStatusRelatorioUseCase");
            UtilTimer.SetTimeout(5000, () => servicoFila.PublicaFila(new PublicaFilaDto(dadosRelatorio, RotasRabbit.RotaRelatoriosProcessando, null, filtroRelatorioDto.CodigoCorrelacao)));
            SentrySdk.CaptureMessage("8 - MonitorarStatusRelatorioUseCase - Publicado na fila Processando -> Não está pronto ou Erro!");
        }
    }
}
