using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IConselhoClasseRecomendacaoRepository
    {
        Task<IEnumerable<ConselhoClasseRecomendacao>> ObterTodos();
    }
}
