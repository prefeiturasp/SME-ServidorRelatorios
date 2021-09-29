using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioBoletimEscolarDetalhadoEscolaAquiUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
