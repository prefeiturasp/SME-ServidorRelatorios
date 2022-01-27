using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioDevolutivasSincronoUseCase
    {
        Task GerarRelatorioSincrono(FiltroRelatorioDto request);
    }
}
