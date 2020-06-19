using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioConselhoClasseAtaFinalUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
