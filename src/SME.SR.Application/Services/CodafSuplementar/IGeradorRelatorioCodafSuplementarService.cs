using SME.SR.Infra.Dtos.Codaf;
using System.IO;

namespace SME.SR.Application.Services.CodafSuplementar
{
    public interface IGeradorRelatorioCodafSuplementarService
    {
        MemoryStream GerarRelatorio(RelatorioCodafDto dadosRelatorio);
    }
}