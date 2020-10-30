using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioSondagemComponentesPorTurmaUseCase
    {
        Task<string> Executar(FiltroRelatorioSincronoDto request);
    }
}
