using MediatR;
using SME.SR.Application;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP
{
    public class RelatorioPlanoAulaUseCase : IRelatorioPlanoAulaUseCase
    {
        private readonly IMediator mediator;

        public RelatorioPlanoAulaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<ObterPlanoAulaIdDto>();

            await mediator.Send(new ObterPlanoAulaQuery(filtros.PlanoAulaId));
        }
    }
}
