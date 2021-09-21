using MediatR;
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
            var dadosRelatorio = filtroRelatorioDto.ObterObjetoFiltro<DadosRelatorioDto>();

            var detalhesRelatorio = await mediator.Send(new ObterDetalhesRelatorioQuery(dadosRelatorio.RequisicaoId, dadosRelatorio.JSessionId));

            if (detalhesRelatorio != null && detalhesRelatorio.Pronto)
            {
                if (await mediator.Send(new SalvarRelatorioJasperLocalCommand(dadosRelatorio.JSessionId, dadosRelatorio.RequisicaoId, dadosRelatorio.ExportacaoId, dadosRelatorio.CodigoCorrelacao)))
                {
                    await servicoFila.PublicaFila(new PublicaFilaDto(dadosRelatorio, RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, filtroRelatorioDto.CodigoCorrelacao));

                }
                else await PublicarNovamenteNaFila(filtroRelatorioDto, dadosRelatorio);

            }
            else
            {
                await PublicarNovamenteNaFila(filtroRelatorioDto, dadosRelatorio);
            }
        }

        private async Task PublicarNovamenteNaFila(FiltroRelatorioDto filtroRelatorioDto, DadosRelatorioDto dadosRelatorio)
        {
            await Task.Run(() => UtilTimer.SetTimeout(5000, async () => await servicoFila.PublicaFila(new PublicaFilaDto(dadosRelatorio, filtroRelatorioDto.RotaProcessando, null, filtroRelatorioDto.CodigoCorrelacao))));
        }
    }
}
