using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioNotasEConceitosFinaisUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
