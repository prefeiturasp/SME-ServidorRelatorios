using Microsoft.AspNetCore.Mvc;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using SME.SR.Workers.SGP.Filters;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Controllers
{
    [ApiController]
    [ChaveIntegracaoSrApi]
    [Route("api/v1/cdep")]
    public class CDEPController : ControllerBase
    {
        [HttpPost("controle-livros-emprestados")]
        public async Task<IActionResult> RelatorioControleLivrosEmprestados([FromBody] FiltroRelatorioSincronoDto request, 
            [FromServices] IRelatorioControleLivrosEmprestadosUseCase relatorioControleLivrosEmprestados)
        {
            var file = await relatorioControleLivrosEmprestados.Executar(request);
            return File(file, "application/vnd.ms-excel", "relatorio.xls", enableRangeProcessing: true);
        }
    }
}
