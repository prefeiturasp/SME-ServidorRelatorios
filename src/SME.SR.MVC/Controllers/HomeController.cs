using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SME.SR.Application;
using SME.SR.Infra.Dtos.Relatorios.ConselhoClasse;
using System;
using System.Threading.Tasks;

namespace SME.SR.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
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
