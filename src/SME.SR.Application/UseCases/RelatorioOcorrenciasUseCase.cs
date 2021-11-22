using MediatR;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioOcorrenciasUseCase : IRelatorioOcorrenciasUseCase
    {
        public readonly IMediator mediator;
        public RelatorioOcorrenciasUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public  async Task Executar(FiltroRelatorioDto request)
        {
            request.RotaErro = RotasRabbitSGP.RotaRelatoriosComErroRelatorioOcorrencia;

            var filtroRelatorio = request.ObterObjetoFiltro<FiltroImpressaoOcorrenciaDto>();

            var retornoRelatorio = await mediator.Send(new ObterDadosRelatorioOcorrenciaQuery(filtroRelatorio));

            await mediator.Send(new GerarRelatorioOcorrenciasCommand(retornoRelatorio, request.CodigoCorrelacao));
        }
    }
}
