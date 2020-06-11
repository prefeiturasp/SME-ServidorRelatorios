using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application.Interfaces
{
    public interface IRelatorioGamesUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
