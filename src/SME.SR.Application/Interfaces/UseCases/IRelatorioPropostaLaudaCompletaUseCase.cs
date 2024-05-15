using System.Threading.Tasks;

namespace SME.SR.Application.Interfaces
{
    public interface IRelatorioPropostaLaudaCompletaUseCase
    {
        Task<string> Executar(long propostaId);
    }
}
