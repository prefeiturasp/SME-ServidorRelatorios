using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
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

        //[HttpGet("relatorios/alunos/dados")]
        //[Action("relatorios/alunos/dados")]
        //public async Task<bool> RelatorioDadosAluno([FromQuery] JObject request, [FromServices] IMediator mediator)
        //{
        //    Console.WriteLine("[ INFO ] Nome da action: " + request["action"]);
        //    return await RelatorioDadosAlunoUseCase.Executar(mediator);
        //}
        public WorkerSGPController(IRelatorioGamesUseCase relatorioGamesUseCase)
        {
            this.relatorioGamesUseCase = relatorioGamesUseCase ?? throw new ArgumentNullException(nameof(relatorioGamesUseCase));
        }
        [HttpGet("relatorios/alunos")]
        [Action("relatorios/alunos")]
        public async Task<bool> RelatorioGames([FromQuery] JObject request)
        {
            await relatorioGamesUseCase.Executar(request);
            //Console.WriteLine("[ INFO ] Nome da action: " + request["action"]);
            //return await RelatorioDadosAlunoUseCase.Executar(mediator);
            return true;
        }
    }
}
