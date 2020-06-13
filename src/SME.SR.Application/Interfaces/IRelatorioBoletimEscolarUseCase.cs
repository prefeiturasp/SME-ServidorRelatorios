using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP
{
    public interface IRelatorioBoletimEscolarUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
