using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioSondagemMatemicaConsolidadoUseCase
    {
        Task<string> Executar(FiltroRelatorioSincronoDto request);
    }
}
