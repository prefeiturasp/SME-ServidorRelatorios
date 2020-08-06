using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioRecuperacaoParalelaUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
