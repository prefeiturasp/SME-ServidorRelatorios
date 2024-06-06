using System.Threading.Tasks;

namespace SME.SR.Application.Interfaces
{
    public interface IRelatorioPropostaLaudaDePublicacaoUseCase
    {
        Task<string> Executar(long propostaId);
    }
}
