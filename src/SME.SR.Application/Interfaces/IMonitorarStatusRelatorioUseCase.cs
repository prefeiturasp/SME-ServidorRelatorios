using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application.Interfaces
{
    public interface IMonitorarStatusRelatorioUseCase
    {
        Task Executar(FiltroRelatorioDto filtroRelatorioDto);
    }
}
