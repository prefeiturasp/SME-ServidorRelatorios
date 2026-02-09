using SME.SR.Infra.Dtos.Codaf;
using System.IO;

namespace SME.SR.Application.Services.Codaf
{
    public interface IGeradorRelatorioCodafService
    {
        MemoryStream GerarRelatorio(RelatorioCodafDto dadosRelatorio);
    }
}