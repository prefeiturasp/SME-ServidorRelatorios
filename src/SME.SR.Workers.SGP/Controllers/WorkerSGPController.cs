using Microsoft.AspNetCore.Mvc;
using SME.SR.Infra;
using SME.SR.Workers.SGP.Commons.Attributes;
using System;
using System.Threading.Tasks;


namespace SME.SR.Workers.SGP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Worker("sme.sr.workers.sgp")]
    public class WorkerSGPController : ControllerBase
    {
        private readonly IRelatorioGamesUseCase relatorioGamesUseCase;

        public WorkerSGPController(IRelatorioGamesUseCase relatorioGamesUseCase)
        {
            this.relatorioGamesUseCase = relatorioGamesUseCase ?? throw new ArgumentNullException(nameof(relatorioGamesUseCase));
        }
		
        [HttpGet("relatorio/dadosaluno")]
        [Action("relatorio/dadosaluno")]
        public async Task<bool> RelatorioDadosAluno([FromQuery] JObject request, [FromServices] IMediator mediator)
        {
            Console.WriteLine("[ INFO ] Nome da action: " + request["action"]);
            return await RelatorioDadosAlunoUseCase.Executar(mediator);
        }
		
        [HttpGet("relatorio/conselhoclasseturma")]
        [Action("relatorio/conselhoclasseturma")]
        public async Task<string> RelatorioConselhoClasseTurma([FromQuery] JObject request, [FromServices] IMediator mediator)
        {
            Console.WriteLine("[ INFO ] Nome da action: " + request["action"]);
            return await RelatorioConselhoClasseTurmaUseCase.Executar(mediator, request);
        }

        [HttpGet("relatorio/conselhoclassealuno")]
        [Action("relatorio/conselhoclassealuno")]
        public async Task<string> RelatorioConselhoClasseAluno([FromQuery] JObject request, [FromServices] IMediator mediator)
        {
            Console.WriteLine("[ INFO ] Nome da action: " + request["action"]);
            return await RelatorioConselhoClasseAlunoUseCase.Executar(mediator,request);
        }

        [HttpGet("relatorios/alunos")]
        [Action("relatorios/alunos")]
        public async Task<bool> RelatorioGames([FromQuery] FiltroRelatorioDto request)
        {
            await relatorioGamesUseCase.Executar(request);
            return true;
        }
    }
}
