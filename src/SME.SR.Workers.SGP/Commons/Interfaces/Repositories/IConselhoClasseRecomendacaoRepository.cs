using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Commons
{
    public interface IConselhoClasseRecomendacaoRepository
    {
        Task<IEnumerable<ConselhoClasseRecomendacao>> ObterTodos();
    }
}
