using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IQuestionarioRepository
    {
        Task<long> ObterQuestionarioIdPorTipo(int tipoQuestionario);
        Task<IEnumerable<Questao>> ObterQuestoesPorQuestionarioId(long questionarioId);
    }
}