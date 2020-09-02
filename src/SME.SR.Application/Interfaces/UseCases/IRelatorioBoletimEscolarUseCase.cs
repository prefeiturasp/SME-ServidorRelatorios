using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioBoletimEscolarUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
