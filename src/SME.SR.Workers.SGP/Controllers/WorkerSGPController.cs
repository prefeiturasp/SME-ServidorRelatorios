using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SME.SR.Workers.SGP.Commons.Attributes;
using Microsoft.Extensions.DependencyInjection;


namespace SME.SR.Workers.SGP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Worker("sme.sr.workers.sgp")]
    public class WorkerSGPController : ControllerBase
    {
        //[HttpGet("relatorios/alunos/dados")]
        //[Action("relatorios/alunos/dados")]
        //public async Task<bool> RelatorioDadosAluno([FromQuery] JObject request, [FromServices] IMediator mediator)
        //{
        //    Console.WriteLine("[ INFO ] Nome da action: " + request["action"]);
        //    return await RelatorioDadosAlunoUseCase.Executar(mediator);
        //}

        [HttpGet("relatorios/games/")]
        [Action("relatorios/games/")]
        public async Task<bool> RelatorioGames([FromQuery] JObject request, [FromServices]IRelatorioGamesUseCase )
        {
            //Console.WriteLine("[ INFO ] Nome da action: " + request["action"]);
            //return await RelatorioDadosAlunoUseCase.Executar(mediator);
            return true;
        }
    }
}
