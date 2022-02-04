using Microsoft.AspNetCore.Mvc;
using SME.SR.Application;
using SME.SR.Workers.SGP.Filters;
using System;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Controllers
{
    [Route("api/v1/downloads/sgp")]
    [ApiController]
    public class SgpDownloadController : Controller
    {
        [HttpGet("pdf/{relatorionome}/{correlacaoId}")]
        public async Task<IActionResult> DownloadPdf(Guid correlacaoId, string relatorioNome,
            [FromServices] IDownloadRelatorioUseCase downloadPdfRelatorioUseCase)
        {
            return File(await downloadPdfRelatorioUseCase.Executar(correlacaoId, ".pdf", "relatorios"), "application/pdf",
                $"{relatorioNome}");
        }

        [HttpGet("pdfsincrono/{relatorionome}/{correlacaoId}")]
        public async Task<IActionResult> DownloadPdfSincrono(Guid correlacaoId, string relatorioNome,[FromServices] IDownloadRelatorioUseCase downloadPdfRelatorioUseCase)
        {
            return File(await downloadPdfRelatorioUseCase.Executar(correlacaoId, ".pdf", "relatoriossincronos"), "application/pdf",
                $"{relatorioNome}");
        }

        [HttpGet("xlsx/{relatorionome}/{correlacaoId}")]
        public async Task<IActionResult> DownloadExcel(Guid correlacaoId, string relatorioNome,
            [FromServices] IDownloadRelatorioUseCase downloadPdfRelatorioUseCase)
        {
            return File(await downloadPdfRelatorioUseCase.Executar(correlacaoId, ".xlsx", "relatorios"),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{relatorioNome}");
        }

        [HttpGet("html/{relatorionome}/{correlacaoId}")]
        public async Task<IActionResult> Downloadhtml(Guid correlacaoId, string relatorioNome,
            [FromServices] IDownloadRelatorioUseCase downloadPdfRelatorioUseCase)
        {
            return File(await downloadPdfRelatorioUseCase.Executar(correlacaoId, ".html", "relatorios"),
                "text/html", $"{relatorioNome}.html");
        }
    }
}