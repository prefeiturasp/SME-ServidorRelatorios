using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterDadosPlanejamentoDiarioBordoComComponenteQuery : IRequest<IEnumerable<TurmaPlanejamentoDiarioInfantilDto>>
    {

        public ObterDadosPlanejamentoDiarioBordoComComponenteQuery(FiltroRelatorioPlanejamentoDiarioDto parametros)
        {
            Parametros = parametros;
        }

        public FiltroRelatorioPlanejamentoDiarioDto Parametros { get; set; }
    }
}
