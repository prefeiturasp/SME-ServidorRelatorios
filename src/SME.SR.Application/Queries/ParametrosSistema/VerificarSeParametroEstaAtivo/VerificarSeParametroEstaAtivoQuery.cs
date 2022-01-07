using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class VerificarSeParametroEstaAtivoQuery : IRequest<ParametroSistemaAnoSituacaoDto>
    {
        public VerificarSeParametroEstaAtivoQuery(TipoParametroSistema tipoParametro)
        {
            TipoParametro = tipoParametro;
        }

        public TipoParametroSistema TipoParametro { get; set; }
    }
}
