using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SME.SR.Infra.Dtos.Requisicao;
using SME.SR.JRSClient.Interfaces;


namespace SME.SR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TesteController : ControllerBase
    {
        public async Task<IActionResult> Get([FromServices] IInformacaoServidorService informacaoServidorRequisicao)
        {
            return Ok(await informacaoServidorRequisicao.Obter());
        }
        [HttpGet("status")]
        public async Task<IActionResult> ReportStatus([FromServices] ILoginService loginService)
        {
            return Ok(await loginService.ObterReportStatus());
        }

        //OK
        [HttpGet("postasync")]
        public async Task<IActionResult> PostAsync([FromServices] IExecucaoRelatorioService execucaoRelatorioService)
        {
            return Ok(await execucaoRelatorioService.PostAsync(
                new ExecucaoRelatorioRequisicaoDto()
                {
                    Async = true,
                    FormatoSaida = "pdf",
                    IgnorarCache = true,
                    IgnorarPaginacao = true,
                    Interativo = false,
                    Parametros = new ParametrosRelatorioDto()
                    {
                        ParametrosRelatorio = new ParametroDto[] {
                                 new ParametroDto()
                                 {
                                      Nome = "Parametro",
                                       Valor = new string[] { "a" }
                                 }
                               }
                    },
                    SalvarSnapshot = false,
                    UnidadeRelatorioUri = "/Nova_pasta/Report",
                    Paginas = null
                }));
        }

        [HttpGet("pool/{requestId}")]
        public async Task<IActionResult> Pool([FromServices] IExecucaoRelatorioService execucaoRelatorioService, Guid requestId)
        {
            return Ok(await execucaoRelatorioService.ObterPool(requestId));
        }

        [HttpGet("detalhes/{requestId}")]
        public async Task<IActionResult> Detalhes([FromServices] IExecucaoRelatorioService execucaoRelatorioService, Guid requestId)
        {
            return Ok(await execucaoRelatorioService.ObterDetalhes(requestId));
        }

        [HttpGet("saida/{requestId}/exports/{exportID}/outputResource")]
        public async Task<IActionResult> Saida([FromServices] IExecucaoRelatorioService execucaoRelatorioService, Guid requestId, Guid exportID)
        {
            return Ok(await execucaoRelatorioService.ObterSaida(requestId, exportID));
        }

        [HttpGet("anexo/{requestID}/exports/{exportID}/attachments/{fileName}")]
        public async Task<IActionResult> Anexo([FromServices] IExecucaoRelatorioService execucaoRelatorioService, Guid requestId, Guid exportID, string fileName)
        {
            return Ok(await execucaoRelatorioService.ObterAnexos(requestId, exportID, fileName));
        }

        [HttpGet("exportar/{requestID}/exports/")]
        public async Task<IActionResult> Exportar([FromServices] IExecucaoRelatorioService execucaoRelatorioService, Guid requestId)
        {
            return Ok(await execucaoRelatorioService.PostExportacao(requestId,
                new ExportacaoRelatorioRequisicaoDto()
                {
                    AnexosPrefixo = "./images/",
                    FormatoSaida = "html",
                    Paginas = "10-20"
                }));
        }

        [HttpGet("parametros/{requestID}")]
        public async Task<IActionResult> Parametros([FromServices] IExecucaoRelatorioService execucaoRelatorioService, Guid requestId)
        {
            return Ok(await execucaoRelatorioService.PostModificarParametros(requestId,
                new ModificarParametrosRelatorioRequisicaoDto()
                {
                    Parametros = new ParametroDto[]
                    {
                         new ParametroDto()
                         {
                              Nome = "a",
                              Valor = new string[] { "teste" }
                         }
                    }
                }));
        }

        [HttpGet("poolExport/{requestID}/exports/{exportID}")]
        public async Task<IActionResult> PoolExportacao([FromServices] IExecucaoRelatorioService execucaoRelatorioService, Guid requestId, Guid exportID)
        {
            return Ok(await execucaoRelatorioService.ObterPoolExportacao(requestId, exportID));
        }

        [HttpGet("running")]
        public async Task<IActionResult> EmExecucao([FromServices] IExecucaoRelatorioService execucaoRelatorioService)
        {
            return Ok(await execucaoRelatorioService.ObterRelatoriosTarefasEmAndamento(
                new RelatoriosTarefasEmAndamentoRequisicaoDto()
                {
                    DataFim = DateTime.Today,
                    DataInicio = DateTime.MinValue,
                    RelatorioUri = "/Nova_pasta/Report",
                    TarefaId = "",
                    TarefaLabel = "",
                    UsuarioNome = "Teste"
                }
                ));
        }

        [HttpGet("stop/{requestId}")]
        public async Task<IActionResult> Parar([FromServices] IExecucaoRelatorioService execucaoRelatorioService, Guid requestId)
        {
            return Ok(await execucaoRelatorioService.InterromperRelatoriosTarefas(requestId));
        }
    }
}