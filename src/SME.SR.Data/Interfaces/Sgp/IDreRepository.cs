using SME.SR.Data.Models;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IDreRepository
    {
        Task<Dre> ObterPorCodigo(string dreCodigo);
    }
}
