using Microsoft.AspNetCore.Mvc;
using SME.SR.Application;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Controllers
{
    [ApiController]
    [Route("api/v1/relatorios/sincronos")]
    public class RelatoriosSincronosSgpController : ControllerBase
    {
        [HttpPost("faltas-frequencia")]
        public async Task<Guid> RelatorioFaltasFrequencias([FromBody] FiltroRelatorioDto request, [FromServices] IRelatorioFaltasFrequenciasUseCase relatorioFaltasFrequenciasUseCase)
        {
            await relatorioFaltasFrequenciasUseCase.Executar(request);
            return request.CodigoCorrelacao;
        }
    }
}
