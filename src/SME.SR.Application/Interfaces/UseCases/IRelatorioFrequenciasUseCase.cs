using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioFrequenciasUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
