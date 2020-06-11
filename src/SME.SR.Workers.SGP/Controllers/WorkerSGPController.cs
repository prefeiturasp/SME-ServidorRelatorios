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
        private readonly IRelatorioBoletimEscolarUseCase relatorioBoletimEscolarUseCase;

        public WorkerSGPController(IRelatorioGamesUseCase relatorioGamesUseCase, IRelatorioBoletimEscolarUseCase relatorioBoletimEscolarUseCase)
        {
            this.relatorioGamesUseCase = relatorioGamesUseCase ?? throw new ArgumentNullException(nameof(relatorioGamesUseCase));
            this.relatorioBoletimEscolarUseCase = relatorioBoletimEscolarUseCase ?? throw new ArgumentNullException(nameof(relatorioBoletimEscolarUseCase));
        }

        [HttpGet("relatorios/alunos")]
        [Action("relatorios/alunos")]
        public async Task<bool> RelatorioGames([FromQuery] FiltroRelatorioDto request)
        {
            await relatorioGamesUseCase.Executar(request);
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
