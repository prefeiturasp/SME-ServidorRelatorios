using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioPlanoAulaUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
