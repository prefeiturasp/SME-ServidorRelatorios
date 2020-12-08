using MediatR;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioAdesaoAppUseCase : IRelatorioAdesaoAppUseCase
    {
        private readonly IMediator mediator;

        public RelatorioAdesaoAppUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var relatorioFiltros = request.ObterObjetoFiltro<AdesaoAEFiltroDto>();

            var listaConsolida = await mediator.Send(new ObterValoresConsolidadosAdesaoAppQuery(relatorioFiltros.DreCodigo, relatorioFiltros.UeCodigo));

            if (!listaConsolida.Any())
            {
                //Verificar oq fazer caso a lista não tenha retorno
            }

            var listaRetorno = await mediator.Send(new ObterListaRelatorioAdessaoAEQuery(listaConsolida, relatorioFiltros));
            
        }
    }
}
