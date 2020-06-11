using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterParametroSistemaPorTipoQuery :IRequest<string>
    {
        public TipoParametroSistema TipoParametro { get; set; }
    }
}
