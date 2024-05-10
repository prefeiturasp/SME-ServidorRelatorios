using Microsoft.AspNetCore.Mvc;
using SME.SR.Application.Interfaces;
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
        public async Task<string> ObterRelatorioProstaDeLaudaDePublicacao(long propostaId, [FromServices] IRelatorioPropostaLaudaDePublicacaoUseCase useCase)
        {
            return await useCase.Executar(propostaId);
        }

        [HttpGet("prosposta/{propostaId}/lauda-completa")]
        public async Task<string> ObterRelatorioProstaDeLaudaCompleta(long propostaId, [FromServices] IRelatorioPropostaLaudaCompletaUseCase useCase)
        {
            return await useCase.Executar(propostaId);
        }
    }
}
