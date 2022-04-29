using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IDreRepository
    {
        Task<Dre> ObterPorCodigo(string dreCodigo);
        Task<IEnumerable<Dre>> ObterTodas();
        Task<Dre> ObterPorId(long dreId);
        Task<DreUe> ObterDreUePorTurmaCodigo(string turmaCodigo);
    }
}
