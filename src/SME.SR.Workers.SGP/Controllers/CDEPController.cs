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
        public async Task<string> RelatorioControleLivrosEmprestados([FromBody] FiltroRelatorioSincronoDto request, [FromServices] IRelatorioControleLivrosEmprestadosUseCase relatorioControleLivrosEmprestados)
        {
            return await relatorioControleLivrosEmprestados.Executar(request);
        }
    }
}
