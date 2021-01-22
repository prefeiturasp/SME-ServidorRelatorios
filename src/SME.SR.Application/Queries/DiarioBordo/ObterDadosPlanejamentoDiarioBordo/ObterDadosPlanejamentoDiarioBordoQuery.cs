using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterDadosPlanejamentoDiarioBordoQuery : IRequest<IEnumerable<TurmaPlanejamentoDiarioDto>>
    {

        public ObterDadosPlanejamentoDiarioBordoQuery(FiltroRelatorioPlanejamentoDiarioDto parametros)
        {
            Parametros = parametros;
        }

        public FiltroRelatorioPlanejamentoDiarioDto Parametros { get; set; }
    }
}
