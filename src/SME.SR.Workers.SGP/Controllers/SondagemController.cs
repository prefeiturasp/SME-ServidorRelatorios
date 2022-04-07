using Microsoft.AspNetCore.Mvc;
using SME.SR.Application;
using SME.SR.Infra;
using SME.SR.Workers.SGP.Filters;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Controllers
{
    [ApiController]
    //[ChaveIntegracaoSrApi]
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
            return await relatorioSondagemMatematicaConsolidadoUseCase.Executar(request);
        }

        [HttpPost("matematica-consolidado-aditivo-multiplicativo")]
        public async Task<string> RelatorioMatemicaConsolidade([FromBody] FiltroRelatorioSincronoDto request, [FromServices] IRelatorioSondagemMatConsolidadoAdtMultiUseCase relatorioSondagemMatConsolidadoAdtMultiUseCase)
        {
            return await relatorioSondagemMatConsolidadoAdtMultiUseCase.Executar(request);
        }

        [HttpPost("portugues-por-turma")]
        public async Task<string> RelatorioSondagemPortuguesPorTurma([FromBody] FiltroRelatorioSincronoDto request, [FromServices] IRelatorioSondagemPortuguesPorTurmaUseCase relatorioSondagemPortuguesPorTurmaUseCase)
        {
            return await relatorioSondagemPortuguesPorTurmaUseCase.Executar(request);
        }

        [HttpPost("portugues-consolidado")]
        public async Task<string> RelatorioPortuguesConsolidado([FromBody] FiltroRelatorioSincronoDto request, [FromServices] IRelatorioSondagemPortuguesConsolidadoUseCase relatorioSondagemPortuguesConsolidadoUseCase)
        {
            return await relatorioSondagemPortuguesConsolidadoUseCase.Executar(request);
        }

        [HttpPost("portugues-por-turma-capacidade-leitura")]
        public async Task<string> RelatorioPortuguesPorTurmaCapacidadeLeitura([FromBody] FiltroRelatorioSincronoDto request, [FromServices] IRelatorioSondagemPtPorTurmaCapLeituraUseCase relatorioSondagemPtPorTurmaCapLeituraUseCase)
        {
            return await relatorioSondagemPtPorTurmaCapLeituraUseCase.Executar(request);
        }

        [HttpPost("portugues-consolidado-leitura-escrita-producao")]
        public async Task<string> RelatorioPortuguesConsolidadoLeituraEscritaProducao([FromBody] FiltroRelatorioSincronoDto request, [FromServices] IRelatorioSondagemPtConsolidadoLeitEscProdUseCase relatorioPtConsolidadoLeituraEscritaProducaoUseCase)
        {
            return await relatorioPtConsolidadoLeituraEscritaProducaoUseCase.Executar(request);
        }
    }
}
