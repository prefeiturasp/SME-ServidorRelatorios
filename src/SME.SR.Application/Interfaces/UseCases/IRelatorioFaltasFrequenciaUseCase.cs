using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioFaltasFrequenciaUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}