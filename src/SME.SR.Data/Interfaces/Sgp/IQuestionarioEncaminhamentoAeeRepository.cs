using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IQuestionarioEncaminhamentoAeeRepository
    {
        Task<long> ObterQuestionarioIdPorTipoESecao(int tipoQuestionario, string nomeComponenteSecao);
    }
}