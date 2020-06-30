using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioHistoricoEscolarUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
