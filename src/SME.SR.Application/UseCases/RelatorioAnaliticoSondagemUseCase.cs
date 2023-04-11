using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioAnaliticoSondagemUseCase : IRelatorioAnaliticoSondagemUseCase
    {
        private readonly IMediator mediator;

        public RelatorioAnaliticoSondagemUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtro = request.ObterObjetoFiltro<FiltroRelatorioAnaliticoSondagemDto>();
            var relatorios = await mediator.Send(new ObterRelatorioAnaliticoSondagemQuery(filtro));
            
            if (relatorios == null || !relatorios.Any())
                throw new NegocioException("Não há dados para o relatório analítico da sondagem.");

            await mediator.Send(new GerarRelatorioAnaliticoDaSondagemExcelCommand(relatorios, filtro.TipoSondagem, request.CodigoCorrelacao));
        }
    }
}
