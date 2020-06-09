using SME.SR.Workers.SGP.Infra;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Commons
{
    public interface IParametroSistemaRepository
    {
        Task<string> ObterValorPorTipo(TipoParametroSistema tipo);
    }
}
