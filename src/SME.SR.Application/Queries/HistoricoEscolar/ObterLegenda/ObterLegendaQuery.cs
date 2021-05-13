using System.Collections.Generic;
using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterLegendaQuery : IRequest<IEnumerable<ConceitoDto>>
    {
        public TipoLegenda TipoLegenda { get; set; }
        public ObterLegendaQuery(TipoLegenda tipoLegenda)
        {
            TipoLegenda = tipoLegenda;
        }
    }
}
