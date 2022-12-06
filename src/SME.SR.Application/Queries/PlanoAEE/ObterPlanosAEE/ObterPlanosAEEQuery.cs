using System.Collections.Generic;
using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterPlanosAEEQuery : IRequest<IEnumerable<PlanosAeeDto>>
    {
        public ObterPlanosAEEQuery(FiltroRelatorioPlanosAeeDto filtro)
        {
            Filtro = filtro;
        }

        public FiltroRelatorioPlanosAeeDto Filtro { get; }
    }
}