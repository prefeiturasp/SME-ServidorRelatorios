using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioPendenciasUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
