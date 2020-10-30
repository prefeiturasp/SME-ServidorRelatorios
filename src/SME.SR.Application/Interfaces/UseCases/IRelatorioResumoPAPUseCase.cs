using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioResumoPAPUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
