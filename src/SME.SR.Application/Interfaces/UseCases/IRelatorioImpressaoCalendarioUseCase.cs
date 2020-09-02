using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioImpressaoCalendarioUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
