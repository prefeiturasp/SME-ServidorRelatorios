using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SME.SR.Workers.SGP.Commons.Attributes;
using Microsoft.Extensions.DependencyInjection;
using SME.SR.Workers.SGP.UseCases;

namespace SME.SR.Workers.SGP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Worker("sme.sr.workers.sgp")]
    public class WorkerSGPController : ControllerBase
    {
        private IHttpContextAccessor HttpContextAccessor;

        [HttpGet("/relatorio-diario-de-classe")]
        [Action("relatorio_diario_de_classe")]
        public async Task RelatorioDiarioDeClasse([FromQuery]JObject request)
        {
            IMediator mediator = HttpContext.RequestServices.GetService<IMediator>();
            await RelatorioDiarioDeClasseUseCase.Executar(mediator);
        }
    }
}