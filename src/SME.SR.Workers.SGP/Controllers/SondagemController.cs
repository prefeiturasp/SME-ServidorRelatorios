using Microsoft.AspNetCore.Mvc;
using SME.SR.Application;
using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SondagemController : ControllerBase
    {

        [HttpPost("matematica-por-turma")]
        public async Task<string> RelatorioSondagemComponentesPorTurma([FromBody] FiltroRelatorioSincronoDto request, [FromServices]IRelatorioSondagemComponentesPorTurmaUseCase relatorioSondagemComponentesPorTurmaUseCase)
        {
            return await relatorioSondagemComponentesPorTurmaUseCase.Executar(request);
        }
        [HttpPost("matematica-consolidado")]        
        public async Task<string> RelatorioMatemicaConsolidade([FromBody] FiltroRelatorioSincronoDto request, [FromServices] IRelatorioSondagemMatemicaConsolidadoUseCase relatorioSondagemMatemicaConsolidadoUseCase)
        {
            return (await relatorioSondagemMatemicaConsolidadoUseCase.Executar(request));
        }

        [HttpPost("matematica-consolidado-aditivo-multiplicativo")]
        public async Task<string> RelatorioMatemicaConsolidade([FromBody] FiltroRelatorioSincronoDto request, [FromServices] IRelatorioSondagemMatConsolidadoAdtMultiUseCase relatorioSondagemMatConsolidadoAdtMultiUseCase)
        {
            return await relatorioSondagemMatConsolidadoAdtMultiUseCase.Executar(request);
        }

        [HttpPost("portugues-por-turma-capacidade-leitura")]
        public async Task<string> RelatorioPortuguesPorTurmaCapacidadeLeitura([FromBody] FiltroRelatorioSincronoDto request, [FromServices] IRelatorioSondagemMatConsolidadoAdtMultiUseCase relatorioSondagemMatConsolidadoAdtMultiUseCase)
        {
            return await relatorioSondagemMatConsolidadoAdtMultiUseCase.Executar(request);
        }

    }
}
