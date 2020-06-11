using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IMonitorarStatusRelatorioUseCase
    {
        Task Executar(FiltroRelatorioDto filtroRelatorioDto);
    }
}
