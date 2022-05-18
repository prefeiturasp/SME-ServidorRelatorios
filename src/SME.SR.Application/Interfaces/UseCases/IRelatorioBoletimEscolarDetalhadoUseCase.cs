using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioBoletimEscolarDetalhadoUseCase
    {
        Task Executar(FiltroRelatorioDto request, bool relatorioEscolaAqui);
    }
}
