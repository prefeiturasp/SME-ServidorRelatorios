using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SME.SR.Workers.SGP.Commons;

namespace SME.SR.Workers.SGP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Worker("sme.sr.workers.sgp")]
    public class WorkerSGPController : ControllerBase
    {
        private IMediator _mediator;

        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        [HttpGet("/relatorio-diario-de-classe")]
        [Action("relatorio_diario_de_classe")]
        public void RelatorioDiarioDeClasse([FromQuery]JObject request)
        {
            Console.WriteLine("[ DEBUG ] Relatorio Diario de Classe requisitado.");
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Ok("Ping <> Pong");
        }
    }
}