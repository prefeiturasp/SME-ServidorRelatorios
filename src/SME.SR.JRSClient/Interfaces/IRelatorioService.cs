using SME.SR.Infra.Dtos;
using System.IO;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Interfaces
{
    public interface IRelatorioService
    {
        Task<Stream> GetRelatorioSincrono(RelatorioSincronoDto Dto);
    }
}
