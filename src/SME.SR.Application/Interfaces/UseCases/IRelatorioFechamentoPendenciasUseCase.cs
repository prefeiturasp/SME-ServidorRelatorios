using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioFechamentoPendenciasUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
