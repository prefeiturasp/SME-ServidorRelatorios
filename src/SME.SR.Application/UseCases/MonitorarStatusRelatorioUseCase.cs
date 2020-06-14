using MediatR;
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
                servicoFila.PublicaFila(new PublicaFilaDto(dadosRelatorio, RotasRabbit.FilaClientsSgp, RotasRabbit.RotaRelatoriosProntosSgp, null, filtroRelatorioDto.CodigoCorrelacao));
            }
            else
            {
                UtilTimer.SetTimeout(5000, () => servicoFila.PublicaFila(new PublicaFilaDto(dadosRelatorio, RotasRabbit.FilaWorkerRelatorios, RotasRabbit.RotaRelatoriosProcessando, null, filtroRelatorioDto.CodigoCorrelacao)));
            }
        }
    }
}
