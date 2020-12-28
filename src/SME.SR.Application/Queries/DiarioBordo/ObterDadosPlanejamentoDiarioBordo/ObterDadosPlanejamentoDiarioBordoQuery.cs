using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterDadosPlanejamentoDiarioBordoQuery : IRequest<IEnumerable<TurmaPlanejamentoDiarioDto>>
    {

        public ObterDadosPlanejamentoDiarioBordoQuery(FiltroRelatorioPlanejamentoDiario parametros)
        {
            Parametros = parametros;
        }

        public FiltroRelatorioPlanejamentoDiario Parametros { get; set; }
    }
}
