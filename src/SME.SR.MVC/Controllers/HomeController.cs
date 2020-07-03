using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SME.SR.Infra.Dtos.Relatorios.ConselhoClasse;
using System;

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

        [HttpGet("relatorios/conselhoclasseatafinal")]
        public IActionResult RelatorioConselhoClasseAtaFinal()
        {
            var model = new ConselhoClasseCabecalhoDto("Nome dre", "NomeUE", DateTime.Now);
            return View(model);
        }
    }
}
