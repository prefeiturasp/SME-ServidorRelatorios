using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IQuestionarioEncaminhamentoNAAPARepository
    {
        Task<long> ObterQuestionarioIdPorTipoESecao(int encaminhamentoNAAPA, string nomeComponenteSecao);
    }
}
