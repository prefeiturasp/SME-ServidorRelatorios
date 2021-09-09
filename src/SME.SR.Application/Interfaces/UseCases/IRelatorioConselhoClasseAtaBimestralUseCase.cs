using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioConselhoClasseAtaBimestralUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
