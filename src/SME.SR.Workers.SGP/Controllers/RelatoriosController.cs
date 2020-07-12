using Microsoft.AspNetCore.Mvc;

namespace SME.SR.Workers.SGP.Controllers
{
    public class RelatoriosController : Controller
    {
        public IActionResult RelatorioFaltasFrequencias()
        {
            return View();
        }
    }
}
