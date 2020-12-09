using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SME.SR.Application;
using SME.SR.Infra;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.MVC.Controllers
{
    public class RelatoriosAEController : Controller
    {
        private readonly ILogger<RelatoriosAEController> _logger;

        public RelatoriosAEController(ILogger<RelatoriosAEController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Adesao([FromServices]IMediator mediator)
        {

            var filtroRelatorio = new AdesaoAEFiltroDto()
            {
                DreCodigo = "109300",
                UeCodigo = "",
                UsuarioRF = "",
                UsuarioNome = "",
                OpcaoListaUsuarios = FiltroRelatorioAEAdesaoEnum.ListarUsuariosNao
            };


            var listaConsolida = await mediator.Send(new ObterValoresConsolidadosAdesaoAppQuery(filtroRelatorio.DreCodigo, filtroRelatorio.UeCodigo));

            if (!listaConsolida.Any())
                throw new NegocioException("Não foram encontrados dados com os filtros informados.");


            var model = await mediator.Send(new ObterListaRelatorioAdessaoAEQuery(listaConsolida, filtroRelatorio));


            return View("RelatorioAdesao", model);
        }

    }
}

