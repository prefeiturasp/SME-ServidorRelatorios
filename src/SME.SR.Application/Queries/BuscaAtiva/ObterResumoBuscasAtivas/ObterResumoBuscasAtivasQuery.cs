using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterResumoBuscasAtivasQuery : IRequest<IEnumerable<BuscaAtivaSimplesDto>>
    {
        public ObterResumoBuscasAtivasQuery(FiltroRelatorioBuscasAtivasDto filtro)
        {
            Filtro = filtro;
        }

        public FiltroRelatorioBuscasAtivasDto Filtro { get; }
    }
}
