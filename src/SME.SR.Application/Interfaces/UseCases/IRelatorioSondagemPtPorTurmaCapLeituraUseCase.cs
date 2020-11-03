using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioSondagemPtPorTurmaCapLeituraUseCase
    {
        Task<string> Executar(FiltroRelatorioSincronoDto request);
    }
}
