using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioAtribuicaoCJUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
