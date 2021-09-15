using MediatR;
using Microsoft.AspNetCore.Mvc;
using SME.SR.Application;
using SME.SR.Infra;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.MVC.Controllers
{
    public class RelatorioAtaBimestralController : Controller
    {
        private readonly IMediator mediator;

        public RelatorioAtaBimestralController(IMediator mediator)
        {

            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("relatorio-ata-bimestral")]
        public async Task<IActionResult> RelatorioAtaFinal()
        {

            var filtro = new FiltroConselhoClasseAtaBimestralDto()
            {
                AnoLetivo = 2021,
                TurmasCodigo = new[] { "2317820" },
                Bimestre = 1
            };

            var mensagensErro = new StringBuilder();
            var relatoriosTurmas = await mediator.Send(new ObterRelatorioConselhoClasseAtaBimestralPdfQuery(filtro));

            //var rel = relatoriosTurmas.Where(a => a.GruposMatriz)
            return View("RelatorioAtasComColunaFinal", relatoriosTurmas[2]);
        }
    }
}

