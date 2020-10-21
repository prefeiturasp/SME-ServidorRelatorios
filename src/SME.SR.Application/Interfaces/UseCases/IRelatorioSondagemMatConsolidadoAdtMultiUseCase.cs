using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioSondagemMatConsolidadoAdtMultiUseCase 
    {
        Task<string> Executar(FiltroRelatorioSincronoDto request);
    }
}
