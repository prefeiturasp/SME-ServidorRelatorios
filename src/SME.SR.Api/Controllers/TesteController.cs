using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
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

        [HttpGet("jobs")]
        public async Task<IActionResult> ObterTrabalhos([FromServices] ITrabalhoService trabalhoService)
        {
            return Ok(await trabalhoService.ObterTrabalhosRelatorios());
        }

        [HttpGet("jobs/{TrabalhoId}")]
        public async Task<IActionResult> ObterTrabalhos(int TrabalhoId,[FromServices] ITrabalhoService trabalhoService)
        {
            return Ok(await trabalhoService.ObterTrabalhoRelatorioPorId(TrabalhoId));
        }

        [HttpPost("jobs/{TrabalhoId}")]
        public async Task<IActionResult> AlterarDefinicao(int TrabalhoId, [FromServices] ITrabalhoService trabalhoService)
        {
            var definicao = await trabalhoService.ObterTrabalhoRelatorioPorId(TrabalhoId);

            definicao.Gatilhos.GatilhoSimples.DataInicio = DateTime.Now.AddDays(1);

            return Ok(await trabalhoService.AlterarDefinicaoTrabalho(TrabalhoId, definicao));
        }

        [HttpGet("jobs/{TrabalhoId}/state")]
        public async Task<IActionResult> ObterEstadoTrabalho(int TrabalhoId, [FromServices] ITrabalhoService trabalhoService)
        {
            return Ok(await trabalhoService.ObterTrabalhoEstado(TrabalhoId));
        }

        [HttpPost("jobs/pause")]
        public async Task<IActionResult> PausarTrabalho([FromServices] ITrabalhoService trabalhoService)
        {
            var lista = new TrabalhoListaIdsDto();

            lista.TrabalhoIds = (await trabalhoService.ObterTrabalhosRelatorios()).TrabalhoResumos.Select(x => x.Id);
            
            return Ok(await trabalhoService.PausarTrabalhos(lista));
        }

        [HttpPost("jobs/resume")]
        public async Task<IActionResult> IniciarTrabalho([FromServices] ITrabalhoService trabalhoService)
        {
            var lista = new TrabalhoListaIdsDto();

            lista.TrabalhoIds = (await trabalhoService.ObterTrabalhosRelatorios()).TrabalhoResumos.Select(x => x.Id);
            
            return Ok(await trabalhoService.InciarTrabalhos(lista));
        }

        [HttpPost("jobs/restart")]
        public async Task<IActionResult> ReiniciarTrabalho([FromServices] ITrabalhoService trabalhoService)
        {
            var lista = new TrabalhoListaIdsDto();

            lista.TrabalhoIds = (await trabalhoService.ObterTrabalhosRelatorios()).TrabalhoResumos.Select(x => x.Id);
            
            return Ok(await trabalhoService.ReinicarTrabalhosFalhados(lista));
        }

        [HttpDelete("jobs/{trabalhoId}")]
        public async Task<IActionResult> DeletarTrabalhoPorId(int trabalhoId, [FromServices] ITrabalhoService trabalhoService)
        {
            return Ok(await trabalhoService.DeletarTrabalhoPorId(trabalhoId));
        }

        [HttpDelete("jobs")]
        public async Task<IActionResult> DeletarTrabalhosPorListaId([FromQuery]IEnumerable<int> ids, [FromServices] ITrabalhoService trabalhoService)
        {
            return Ok(await trabalhoService.DeletarTrabalhosPorListaId(ids));
        }

        [HttpPut("jobs")]
        public async Task<IActionResult> AgendarTrabalhos([FromBody]object trabalhoDefinicaoJson,[FromServices] ITrabalhoService trabalhoService)
        {
            var trabalhoDefinicaoDto = JsonConvert.DeserializeObject<TrabalhoDefinicaoDto>(trabalhoDefinicaoJson.ToString());

            return Ok(await trabalhoService.AgendarTrabalhoRelatorio(trabalhoDefinicaoDto));
        }

        [HttpGet("jobsc")]
        public async Task<IActionResult> ObterTrabalhosInformandoRelatorio([FromServices] ITrabalhoService trabalhoService)
        {
            return Ok(await trabalhoService.ObterTrabalhosRelatorios("/temp"));
        }

        [HttpGet("jobs/filter")]
        public async Task<IActionResult> ObterTrabalhosPorFiltro([FromServices]ITrabalhoService trabalhoService)
        {
            var filtro = new TrabalhoFiltroDto
            {
                Ascendente = true,
                Limite = 2
            };

            var model = new TrabalhoModelDto();

            return Ok(await trabalhoService.ObterTrabalhosRelatorios(filtro, model));
        }

        [HttpPost("jobs/lote/atualizar")]
        public async Task<IActionResult> AtualizarTrabalhoEmLote([FromServices] ITrabalhoService trabalhoService)
        {
            var ids = (await trabalhoService.ObterTrabalhosRelatorios()).TrabalhoResumos.Select(x => x.Id);

            return Ok(await trabalhoService.AtualizarTrabalhosEmLote(ids, false, new TrabalhoDefinicaoDto
            {
                Descricao = "Descricao modificada"
            }));
        }

        [HttpGet("report")]
        public async Task<IActionResult> ObterRelatorioSincrono([FromServices] IRelatorioService relatorioService)
        {
            var dto = new RelatorioSincronoDto
            {
                CaminhoRelatorio = _caminhoRelatorio,
                Formato = Infra.Enumeradores.Enumeradores.FormatoEnum.Pdf,
                IgnorarPaginacao = true,
                Interativo = false,
                Pagina = 1
            };
            
            return Ok(await relatorioService.GetRelatorioSincrono(dto));
        }
    }
}