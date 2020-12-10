using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioAdesaoAppUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
