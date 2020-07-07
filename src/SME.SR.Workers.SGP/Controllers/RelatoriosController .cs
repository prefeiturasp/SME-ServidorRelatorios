using Microsoft.AspNetCore.Mvc;

namespace SME.SR.Workers.SGP.Controllers
{
    [Route("relatorios")]
    public class RelatoriosController : Controller
    {
        [Route("faltas")]
        public IActionResult Index()
        {
            return View("RelatorioFaltas");
        }
    }
}
