using MediatR;
using Microsoft.AspNetCore.Mvc;
using SME.SR.Application.Commands.Conecta.GerarCertificadoCodaf;
using SME.SR.Application.Commands.Conecta.GerarPlanilhaCodaf;
using SME.SR.Application.Commands.Conecta.GerarPlanilhaCodafSuplementar;
using SME.SR.Application.Interfaces;
using SME.SR.Infra.Dtos.Relatorios.Conecta;
using SME.SR.Workers.SGP.Filters;
using System;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Controllers
{
    [ApiController]
    [ChaveIntegracaoSrApi]
    [Route("api/v1/[controller]")]
    public class ConectaController : ControllerBase
    {
        [HttpGet("prosposta/{propostaId}/lauda-publicacao")]
        public async Task<string> ObterRelatorioProstaDeLaudaDePublicacao(long propostaId, [FromServices] IRelatorioPropostaLaudaDePublicacaoUseCase useCase)
        {
            return await useCase.Executar(propostaId);
        }

        [HttpGet("prosposta/{propostaId}/lauda-completa")]
        public async Task<string> ObterRelatorioProstaDeLaudaCompleta(long propostaId, [FromServices] IRelatorioPropostaLaudaCompletaUseCase useCase)
        {
            return await useCase.Executar(propostaId);
        }

        [HttpPost("gerar-certificado-codaf")]
        public async Task<IActionResult> GerarCertificadoCodaf([FromBody] HtmlCertificadoCodafDto request, [FromServices] IMediator mediator)
        {
            var resultado = await mediator.Send(new GerarPdfCertificadoCodafCommand(request));
            return File(resultado, "application/pdf", "certificado-codaf.pdf");
        }

        [HttpPost("codaf/{codafId:long}/gerar-planilha")]
        public async Task<IActionResult> GerarPlanilhaCodaf(long codafId, [FromServices] IMediator mediator)
        {
            var resultado = await mediator.Send(new GerarPlanilhaCodafCommand(codafId));
            return File(resultado, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"relatorio-codaf-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }

        [HttpPost("codaf-suplementar/{codafListaPresencaId:long}/gerar-planilha")]
        public async Task<IActionResult> GerarPlanilhaCodafSuplementar(long codafListaPresencaId, [FromServices] IMediator mediator)
        {
            var resultado = await mediator.Send(new GerarPlanilhaCodafSuplementarCommand(codafListaPresencaId));
            return File(resultado, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"relatorio-codaf-suplementar{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }
    }
}
