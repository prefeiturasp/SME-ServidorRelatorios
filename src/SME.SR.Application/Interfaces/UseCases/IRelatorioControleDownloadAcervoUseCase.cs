using SME.SR.Infra;
using System.IO;
using System.Threading.Tasks;

namespace SME.SR.Application.Interfaces
{
    public interface IRelatorioControleDownloadAcervoUseCase
    {
        Task<MemoryStream> Executar(FiltroRelatorioSincronoDto request);
    }
}
