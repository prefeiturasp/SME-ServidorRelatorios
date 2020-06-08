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

        [HttpGet("relatorios/alunos")]
        [Action("relatorios/alunos")]
        public async Task<bool> RelatorioGames([FromQuery] FiltroRelatorioDto request)
        {
            await relatorioGamesUseCase.Executar(request);
            return true;
        }
    }
}
