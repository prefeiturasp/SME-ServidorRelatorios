using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioSondagemMatematicaConsolidadoUseCase
    {
        Task<string> Executar(FiltroRelatorioSincronoDto request);
    }
}
