using DinkToPdf.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SME.SR.Application.Commands.Conecta.GerarCertificadoCodaf;
using SME.SR.Application.Commands.Conecta.GerarPlanilhaCodaf;
using SME.SR.Application.Interfaces;
using SME.SR.HtmlPdf;
using SME.SR.Infra.Dtos.Codaf;
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

        [HttpPost("gerar-planilha-codaf")]
        public async Task<IActionResult> GerarPlanilhaCodaf([FromBody] RelatorioCodafDto request, [FromServices] IMediator mediator)
        {
            var resultado = await mediator.Send(new GerarPlanilhaCodafCommand(request));
            return File(resultado, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"relatorio-codaf-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }
    }
}
