using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SME.SR.JRSClient.Interfaces;


namespace SME.SR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TesteController : ControllerBase
    {
        private static readonly string _caminhoRelatorio = "Nova_pasta/a";

        public async Task<IActionResult> Get([FromServices] IInformacaoServidorService informacaoServidorRequisicao)
        {
            return Ok(await informacaoServidorRequisicao.Obter());
        }

        [HttpGet("status")]
        public async Task<IActionResult> ReportStatus([FromServices] ILoginService loginService)
        {
            return Ok(await loginService.ObterReportStatus());
        }

        [HttpGet("inputControll")]
        public async Task<IActionResult> InputControll([FromServices] IControleEntradaService controleEntradaService)
        {
            return Ok(await controleEntradaService.ObterControlesEntrada(_caminhoRelatorio, ignorarEstados: false));
        }

        [HttpPut("inputControll")]
        public async Task<IActionResult> InputControllOrder([FromServices] IControleEntradaService controleEntradaService)
        {
            var lista = await controleEntradaService.ObterControlesEntrada(_caminhoRelatorio, ignorarEstados: false);

            return Ok(await controleEntradaService.MudarOrdemControlesEntrada(_caminhoRelatorio, lista));
        }

        [HttpGet("inputControll/values")]
        public async Task<IActionResult> InputControllValues([FromServices] IControleEntradaService controleEntradaService)
        {
            return Ok(await controleEntradaService.ObterEstadosControlesEntrada(_caminhoRelatorio, ignorarCache: false));
        }

        [HttpPost("inputControll")]
        public async Task<IActionResult> SetarValoresControleEntrada([FromServices] IControleEntradaService controleEntradaService)
        {
            var valores = new Dictionary<string, object[]>
            {
                {"t", new string[] { "b"} }
            };

            return Ok(await controleEntradaService.SetarValoresControleEntrada(_caminhoRelatorio, valores, true));
        }

        [HttpGet("resources")]
        public async Task<IActionResult> BuscarRecursos([FromServices] IRecursoService recursoService)
        {
            return Ok(await recursoService.BuscarRepositorio(new Infra.Dtos.BuscaRepositorioRequisicaoDto()
            {
                Query = "adhoc"
            }));
        }

        [HttpGet("resources/details")]
        public async Task<IActionResult> ObterDetalhesRecursoRelatorios([FromServices] IRecursoService recursoService)
        {
            return Ok(await recursoService.ObterDetalhesRecurso("/themes/default/images", false));
        }

        [HttpGet("resources/post")]
        public async Task<IActionResult> Post([FromServices] IRecursoService recursoService)
        {
            return Ok(await recursoService.Post("/themes", true, null, new Infra.Dtos.DetalhesRecursoDto()));
        }

        [HttpGet("resources/put")]
        public async Task<IActionResult> Put([FromServices] IRecursoService recursoService)
        {
            return Ok(await recursoService.Put("/themes", true, null, new Infra.Dtos.DetalhesRecursoDto()));
        }

        [HttpGet("resources/delete")]
        public async Task<IActionResult> Delete([FromServices] IRecursoService recursoService)
        {
            return Ok(await recursoService.Delete("/themes", "teste"));
        }
    }
}