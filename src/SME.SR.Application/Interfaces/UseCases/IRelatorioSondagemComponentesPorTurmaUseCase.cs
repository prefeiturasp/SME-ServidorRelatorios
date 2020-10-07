using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioSondagemComponentesPorTurmaUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
