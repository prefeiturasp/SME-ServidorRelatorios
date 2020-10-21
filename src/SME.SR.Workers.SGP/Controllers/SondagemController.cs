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
        public async Task<string> RelatorioMatematicaConsolidado([FromBody] FiltroRelatorioSincronoDto request, [FromServices] IRelatorioSondagemMatematicaConsolidadoUseCase relatorioSondagemMatematicaConsolidadoUseCase)
        {
            return (await relatorioSondagemMatematicaConsolidadoUseCase.Executar(request));
            
        }
        [HttpPost("portugues-por-turma")]
        public async Task<string> RelatorioSondagemPortuguesPorTurma([FromBody] FiltroRelatorioSincronoDto request, [FromServices] IRelatorioSondagemPortuguesPorTurmaUseCase relatorioSondagemPortuguesPorTurmaUseCase)
        {
            return await relatorioSondagemPortuguesPorTurmaUseCase.Executar(request);
        }
        [HttpPost("portugues-consolidado")]
        public async Task<string> RelatorioPortuguesConsolidado([FromBody] FiltroRelatorioSincronoDto request, [FromServices] IRelatorioSondagemPortuguesConsolidadoUseCase relatorioSondagemPortuguesConsolidadoUseCase)
        {
            return (await relatorioSondagemPortuguesConsolidadoUseCase.Executar(request));
        }
    }
}
