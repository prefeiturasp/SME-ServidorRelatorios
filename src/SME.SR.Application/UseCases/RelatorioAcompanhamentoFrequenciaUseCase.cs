using MediatR;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioAcompanhamentoFrequenciaUseCase : IRelatorioAcompanhamentoFrequenciaUseCase
    {
        public readonly IMediator mediator;
        public RelatorioAcompanhamentoFrequenciaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public Task Executar(FiltroRelatorioDto request)
        {
            request.RotaErro = RotasRabbitSGP.RotaRelatoriosComErroAcompanhamentoFrequencia;

            var filtroRelatorio = request.ObterObjetoFiltro<FiltroAcompanhamentoFrequenciaJustificativaDto>();
            var relatorio = new RelatorioFrequenciaIndividualDto();

            throw new NotImplementedException();
        }
    }
}
