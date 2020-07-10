using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioFaltasFrequenciasUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
