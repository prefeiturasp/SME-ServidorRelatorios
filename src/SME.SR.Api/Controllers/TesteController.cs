using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SME.SR.JRSClient.Interfaces;


namespace SME.SR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TesteController : ControllerBase
    {
        public async Task<IActionResult> Get([FromServices] IInformacaoServidorService informacaoServidorRequisicao)
        {
            return Ok(await informacaoServidorRequisicao.Obter());
        }
        [HttpGet("status")]
        public async Task<IActionResult> ReportStatus([FromServices] ILoginService loginService)
        {
            return Ok(await loginService.ObterReportStatus());
        }
    }
}