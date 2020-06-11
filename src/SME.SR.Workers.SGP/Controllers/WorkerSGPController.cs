using Microsoft.AspNetCore.Mvc;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using SME.SR.Workers.SGP.Commons.Attributes;
using System.Threading.Tasks;


namespace SME.SR.Workers.SGP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Worker("sme.sr.workers.sgp")]
    public class WorkerSGPController : ControllerBase
    {
        [HttpGet("relatorios/alunos")]
        [Action("relatorios/alunos", typeof(IRelatorioGamesUseCase))]
        public async Task<bool> RelatorioGames([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioGamesUseCase relatorioGamesUseCase)
        {
            await relatorioGamesUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorio/conselhoclasseturma")]
        [Action("relatorio/conselhoclasseturma", typeof(IRelatorioConselhoClasseTurmaUseCase))]
        public async Task<bool> RelatorioConselhoClasseTurma([FromQuery] FiltroRelatorioDto request, IRelatorioConselhoClasseTurmaUseCase relatorioConselhoClasseTurmaUseCase)
        {
            await relatorioConselhoClasseTurmaUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorio/conselhoclassealuno")]
        [Action("relatorio/conselhoclassealuno", typeof(IRelatorioConselhoClasseAlunoUseCase))]
        public async Task<bool> RelatorioConselhoClasseAluno([FromQuery] FiltroRelatorioDto request, IRelatorioConselhoClasseAlunoUseCase relatorioConselhoClasseAlunoUseCase)
        {
            await relatorioConselhoClasseAlunoUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/processando")]
        [Action("relatorios/processando", typeof(IMonitorarStatusRelatorioUseCase))]
        public async Task<bool> RelatoriosProcessando([FromQuery] FiltroRelatorioDto request, [FromServices] IMonitorarStatusRelatorioUseCase monitorarStatusRelatorioUseCase)
        {
            await monitorarStatusRelatorioUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/boletimescolar")]
        [Action("relatorios/boletimescolar")]
        public async Task<bool> RelatorioBoletimEscolar([FromQuery] FiltroRelatorioDto request)
        {
            await relatorioBoletimEscolarUseCase.Executar(request);
            return true;
        }
    }
}
