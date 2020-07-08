using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IUeRepository
    {
        Task<Ue> ObterPorCodigo(string ueCodigo);
    }
}
