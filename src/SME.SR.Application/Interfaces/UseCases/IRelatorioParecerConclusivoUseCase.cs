using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioParecerConclusivoUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
