using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IParametroSistemaRepository
    {
        Task<string> ObterValorPorTipo(TipoParametroSistema tipo);
    }
}
