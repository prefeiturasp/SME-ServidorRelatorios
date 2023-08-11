using MediatR;
using SME.SR.Application;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP
{
    public class RelatorioPlanoAnualUseCase : IRelatorioPlanoAnualUseCase
    {
        private readonly IMediator mediator;

        public RelatorioPlanoAnualUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<FiltroPlanoAnualDto>();

            var planoAnualDto = await mediator.Send(new ObterPlanoAnualQuery(filtros.Id));
            planoAnualDto.Usuario = filtros.Usuario;
            planoAnualDto.Objetivos = await mediator.Send(new ObterObjetivosAprendizagemPlanejamentoAnualQuery(filtros.Id));

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioPlanoAnual", planoAnualDto, request.CodigoCorrelacao));
        }
    }
}
