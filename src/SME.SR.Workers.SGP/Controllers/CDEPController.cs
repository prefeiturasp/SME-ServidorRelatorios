using Microsoft.AspNetCore.Mvc;
using SME.SR.Application.Interfaces.UseCases;
using SME.SR.Infra;
using SME.SR.Workers.SGP.Filters;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Controllers
{
    [ApiController]
    [ChaveIntegracaoSrApi]
    [Route("api/v1/[controller]")]
    public class CDEPController : ControllerBase
    {
        [HttpPost("controle-livros-sintetico")]
        public async Task<string> RelatorioControleLivrosEmprestadosSintetico([FromBody] FiltroRelatorioSincronoDto request, [FromServices] IRelatorioControleLivrosEmprestadosUseCase relatorioControleLivrosEmprestados)
        {
            return await relatorioControleLivrosEmprestados.Executar(request);
        }
    }
}
