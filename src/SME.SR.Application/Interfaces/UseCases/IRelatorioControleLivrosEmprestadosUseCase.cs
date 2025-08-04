using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application.Interfaces.UseCases
{
    public interface IRelatorioControleLivrosEmprestadosUseCase
    {
        Task<string> Executar(FiltroRelatorioSincronoDto request);
    }
}
