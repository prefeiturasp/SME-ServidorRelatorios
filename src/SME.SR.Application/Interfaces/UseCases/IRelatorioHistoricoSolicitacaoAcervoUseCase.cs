using SME.SR.Infra;
using System.IO;
using System.Threading.Tasks;

namespace SME.SR.Application.Interfaces.UseCases
{
    public interface IRelatorioHistoricoSolicitacaoAcervoUseCase
    {
        Task<MemoryStream> Executar(FiltroRelatorioSincronoDto request);
    }
}
