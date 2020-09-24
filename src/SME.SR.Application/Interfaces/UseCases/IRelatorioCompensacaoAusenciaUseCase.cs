using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioCompensacaoAusenciaUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
