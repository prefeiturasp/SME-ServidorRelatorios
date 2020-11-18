using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioGraficoPAPUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
