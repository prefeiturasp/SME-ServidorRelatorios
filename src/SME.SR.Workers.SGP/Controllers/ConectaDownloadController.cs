using Microsoft.AspNetCore.Mvc;
using SME.SR.Application;
using System.Threading.Tasks;
using System;

namespace SME.SR.Workers.SGP.Controllers
{
    [Route("api/v1/downloads/conecta")]
    [ApiController]
    public class ConectaDownloadController : Controller
    {
        [HttpGet("doc/{correlacaoId}")]
        public async Task<IActionResult> DownloadDoc(Guid correlacaoId, [FromServices] IDownloadRelatorioUseCase downloadPdfRelatorioUseCase)
        {
            return File(await downloadPdfRelatorioUseCase.Executar(correlacaoId, ".doc", "relatorios"), "application/msword", $"{correlacaoId}");
        }

        [HttpGet("pdf/{correlacaoId}")]
        public async Task<IActionResult> DownloadPdf(Guid correlacaoId, [FromServices] IDownloadRelatorioUseCase downloadPdfRelatorioUseCase)
        {
            return File(await downloadPdfRelatorioUseCase.Executar(correlacaoId, ".pdf", "relatorios"), "application/pdf", $"{correlacaoId}");
        }
    }
}
