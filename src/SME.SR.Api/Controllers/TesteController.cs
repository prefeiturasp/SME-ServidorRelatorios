using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SME.SR.JRSClient.Interfaces;
using SME.SR.JRSClient.Requisicao;

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
    }
}