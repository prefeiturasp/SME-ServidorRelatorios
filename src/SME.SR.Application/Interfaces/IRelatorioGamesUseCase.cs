using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP
{
    public interface IRelatorioGamesUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
