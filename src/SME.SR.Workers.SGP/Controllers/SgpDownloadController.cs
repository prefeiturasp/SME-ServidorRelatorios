using Microsoft.AspNetCore.Mvc;
using SME.SR.Application;
using System;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Controllers
{
    [Route("api/v1/downloads/sgp")]
    [ApiController]
    public class SgpDownloadController : Controller
    {
        [HttpGet("pdf/{correlacaoId}")]
        public async Task<IActionResult> DownloadPdf(Guid correlacaoId, [FromServices] IDownloadRelatorioUseCase downloadPdfRelatorioUseCase)
        {
            return File(await downloadPdfRelatorioUseCase.Executar(correlacaoId, ".pdf"), "application/pdf", $"{correlacaoId}.pdf");
        }
        [HttpGet("xlsx/{correlacaoId}")]
        public async Task<IActionResult> DownloadExcel(Guid correlacaoId, [FromServices] IDownloadRelatorioUseCase downloadPdfRelatorioUseCase)
        {
            return File(await downloadPdfRelatorioUseCase.Executar(correlacaoId, ".xlsx"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{correlacaoId}.xlsx");
        }
    }
}
