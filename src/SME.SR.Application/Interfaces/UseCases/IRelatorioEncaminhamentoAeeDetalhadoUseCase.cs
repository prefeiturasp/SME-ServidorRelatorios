using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application.Interfaces
{
    public interface IRelatorioEncaminhamentoAeeDetalhadoUseCase //: IUseCase
    {
        Task Executar(FiltroRelatorioEncaminhamentoAeeDetalhadoDto request);
    }
}
