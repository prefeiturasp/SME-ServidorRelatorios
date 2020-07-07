using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SME.SR.Infra.Dtos.Relatorios.ConselhoClasse;
using System;

namespace SME.SR.MVC.Controllers
{
    public class RelatoriosController : Controller
    {
        private readonly ILogger<RelatoriosController> _logger;

        public RelatoriosController(ILogger<RelatoriosController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("relatoriofaltas")]
        public IActionResult RelatorioFaltasFrequencias([FromServices] IMediator mediator)
        {
            var model = new ConselhoClasseCabecalhoDto("Nome dre", "NomeUE", DateTime.Now);
            return View(model);
        }
    }
}
