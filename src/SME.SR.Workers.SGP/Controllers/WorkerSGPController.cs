using Microsoft.AspNetCore.Mvc;
using SME.SR.Application.Interfaces;
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
        private readonly IRelatorioConselhoClasseTurmaUseCase relatorioConselhoClasseTurmaUseCase;
        private readonly IRelatorioConselhoClasseAlunoUseCase relatorioConselhoClasseAlunoUseCase;

        public WorkerSGPController(IRelatorioGamesUseCase relatorioGamesUseCase,
                                   IRelatorioConselhoClasseTurmaUseCase relatorioConselhoClasseTurmaUseCase,
                                   IRelatorioConselhoClasseAlunoUseCase relatorioConselhoClasseAlunoUseCase)
        {
            this.relatorioGamesUseCase = relatorioGamesUseCase ?? throw new ArgumentNullException(nameof(relatorioGamesUseCase));
            this.relatorioConselhoClasseTurmaUseCase = relatorioConselhoClasseTurmaUseCase ?? throw new ArgumentNullException(nameof(relatorioConselhoClasseTurmaUseCase));
            this.relatorioConselhoClasseAlunoUseCase = relatorioConselhoClasseAlunoUseCase ?? throw new ArgumentNullException(nameof(relatorioConselhoClasseAlunoUseCase));
        }
		
        [HttpGet("relatorio/conselhoclasseturma")]
        [Action("relatorio/conselhoclasseturma")]
        public async Task<bool> RelatorioConselhoClasseTurma([FromQuery] FiltroRelatorioDto request)
        {
            await relatorioConselhoClasseTurmaUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorio/conselhoclassealuno")]
        [Action("relatorio/conselhoclassealuno")]
        public async Task<bool> RelatorioConselhoClasseAluno([FromQuery] FiltroRelatorioDto request)
        {
            await relatorioConselhoClasseAlunoUseCase.Executar(request);
            return true;
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
