using Microsoft.AspNetCore.Mvc;
using Nest;
using SME.SR.Application;
using SME.SR.Infra;
using SME.SR.Workers.SGP.Filters;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Controllers
{
    [ApiController]
    [ChaveIntegracaoSrApi]
    [Route("api/v1/[controller]")]
    public class ConectaController : ControllerBase
    {
        [HttpGet("prosposta/{propostaId}/lauda-publicacao")]
        public async Task<string> ObterRelatorioProstaDeLaudaDePublicacao(long propostaId, [FromServices] IRelatorioSondagemComponentesPorTurmaUseCase relatorioSondagemComponentesPorTurmaUseCase)
        {
            return await relatorioSondagemComponentesPorTurmaUseCase.Executar(request);
        }
    }
}
