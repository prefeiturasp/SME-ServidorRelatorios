using Microsoft.AspNetCore.Mvc;
using SME.SR.Application.Interfaces;
using SME.SR.Application.Interfaces.UseCases;
using SME.SR.Infra;
using SME.SR.Workers.SGP.Filters;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Controllers
{
    [ApiController]
    [ChaveIntegracaoSrApi]
    [Route("api/v1/cdep")]
    public class CDEPController : ControllerBase
    {
        [HttpPost("controle-livros-emprestados")]
        public async Task<IActionResult> RelatorioControleLivrosEmprestados([FromBody] FiltroRelatorioSincronoDto request,
            [FromServices] IRelatorioControleLivrosEmprestadosUseCase relatorioControleLivrosEmprestados)
        {
            try
            {
                var file = await relatorioControleLivrosEmprestados.Executar(request);
                return File(file,
                      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                      "relatorio.xlsx");
            }
            catch (System.Exception)
            {
                return NoContent();
            }
        }

        [HttpPost("controle-acervo")]
        public async Task<IActionResult> RelatorioControleAcervo([FromBody] FiltroRelatorioSincronoDto request,
            [FromServices] IRelatorioControleAcervoUseCase relatorioControleAcervoUseCase)
        {
            try
            {
                var file = await relatorioControleAcervoUseCase.Executar(request);
                return File(file,
                       "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                       "relatorio.xlsx");
            }
            catch (System.Exception)
            {
                return NoContent();
            }
        }

        [HttpPost("controle-editora")]
        public async Task<IActionResult> RelatorioControleEditora([FromBody] FiltroRelatorioSincronoDto request,
           [FromServices] IRelatorioControleEditoraUseCase relatorioControleEditoraUseCase)
        {
            try
            {
                var file = await relatorioControleEditoraUseCase.Executar(request);
                return File(file,
                      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                      "relatorio.xlsx");
            }
            catch (System.Exception)
            {
                return NoContent();
            }
        }

        [HttpPost("controle-acervo-autor")]
        public async Task<IActionResult> RelatorioControleAcervoAutor([FromBody] FiltroRelatorioSincronoDto request,
            [FromServices] IRelatorioControleAcervoAutorUseCase relatorioControleAcervoAutorUseCase)
        {
            try
            {
                var file = await relatorioControleAcervoAutorUseCase.Executar(request);
                return File(file,
                       "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                       "relatorio.xlsx");

            }
            catch (System.Exception)
            {
                return NoContent();
            }
        }

        [HttpPost("controle-devolucao-livros")]
        public async Task<IActionResult> RelatorioControleDevolucaoLivros([FromBody] FiltroRelatorioSincronoDto request,
            [FromServices] IRelatorioControleDevolucaoLivrosUseCase relatorioControleDevolucaoLivrosUseCase)
        {
            try
            {
                var file = await relatorioControleDevolucaoLivrosUseCase.Executar(request);
                return File(file,
                     "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                     "relatorio.xlsx");

            }
            catch (System.Exception)
            {
                return NoContent();
            }
        }

        [HttpPost("titulos-mais-pesquisados")]
        public async Task<IActionResult> RelatorioTitulosMaisPesquisados([FromBody] FiltroRelatorioSincronoDto request,
            [FromServices] IRelatorioTitulosMaisPesquisadosUseCase relatorioTitulosMaisPesquisadosUseCase)
        {
            try
            {
                var file = await relatorioTitulosMaisPesquisadosUseCase.Executar(request);
                return File(file,
                     "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                     "relatorio.xlsx");
            }
            catch (System.Exception)
            {
                return NoContent();
            }
        }
    }
}
