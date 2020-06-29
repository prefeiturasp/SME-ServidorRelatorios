using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Sentry;
using SME.SR.Application;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.ConselhoClasse;
using SME.SR.Workers.SGP.Commons.Attributes;
using System.Threading.Tasks;


namespace SME.SR.Workers.SGP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Worker("sme.sr.workers.sgp")]
    public class WorkerSGPController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public WorkerSGPController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
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
            using (SentrySdk.Init(configuration.GetValue<string>("Sentry:DSN")))
            {
                SentrySdk.CaptureMessage("4 - relatorio/conselhoclassealuno");
            }
            await relatorioConselhoClasseAlunoUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/processando")]
        [Action("relatorios/processando", typeof(IMonitorarStatusRelatorioUseCase))]
        public async Task<bool> RelatoriosProcessando([FromQuery] FiltroRelatorioDto request, [FromServices] IMonitorarStatusRelatorioUseCase monitorarStatusRelatorioUseCase)
        {
            using (SentrySdk.Init(configuration.GetValue<string>("Sentry:DSN")))
            {
                SentrySdk.CaptureMessage("7 - relatorios/processando");
            }
            await monitorarStatusRelatorioUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/boletimescolar")]
        [Action("relatorios/boletimescolar", typeof(IRelatorioBoletimEscolarUseCase))]
        public async Task<bool> RelatorioBoletimEscolar([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioBoletimEscolarUseCase relatorioBoletimEscolarUseCase)
        {
            await relatorioBoletimEscolarUseCase.Executar(request);
            return true;
        }
        [HttpGet("relatorios/conselhoclasseatafinal")]
        [Action("relatorios/conselhoclasseatafinal", typeof(IRelatorioConselhoClasseAtaFinalUseCase))]
        public async Task<bool> RelatorioConselhoClasseAtaFinal([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioConselhoClasseAtaFinalUseCase relatorioConselhoClasseAtaFinalUseCase)
        {
            await relatorioConselhoClasseAtaFinalUseCase.Executar(request);
            return true;
        }
    }
}
