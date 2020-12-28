using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterPlanejamentoDiarioPlanoAulaQuery : IRequest<IEnumerable<TurmaPlanejamentoDiarioDto>>
    {
        public ObterPlanejamentoDiarioPlanoAulaQuery(FiltroRelatorioPlanejamentoDiarioDto parametros)
        {
            Parametros = parametros;
        }
        public FiltroRelatorioPlanejamentoDiarioDto Parametros { get; set; }
    }
}
