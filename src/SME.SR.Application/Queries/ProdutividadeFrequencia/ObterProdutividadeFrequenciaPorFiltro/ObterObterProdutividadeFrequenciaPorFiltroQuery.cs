using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterObterProdutividadeFrequenciaPorFiltroQuery : IRequest<IEnumerable<ConsolidacaoProdutividadeFrequenciaDto>>
    {
        public ObterObterProdutividadeFrequenciaPorFiltroQuery(FiltroRelatorioProdutividadeFrequenciaDto filtro)
        {
            Filtro = filtro;
        }

        public FiltroRelatorioProdutividadeFrequenciaDto Filtro { get; }
    }
}
