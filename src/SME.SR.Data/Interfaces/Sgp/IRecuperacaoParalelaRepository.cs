using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IRecuperacaoParalelaRepository
    {
        Task<RecuperacaoParalelaPeriodo> ObterPeriodoPorId(long id);
    }
}
