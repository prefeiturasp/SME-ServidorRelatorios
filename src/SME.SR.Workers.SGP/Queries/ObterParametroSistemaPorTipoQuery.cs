using MediatR;
using SME.SR.Workers.SGP.Infra;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterParametroSistemaPorTipoQuery :IRequest<string>
    {
        public TipoParametroSistema TipoParametro { get; set; }
    }
}
