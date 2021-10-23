using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SME.SR.Application;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using SME.SR.Workers.SGP.Filters;
using System;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Controllers
{
    [ApiController]
    [ChaveIntegracaoSrApi]
    [Route("api/v1/relatorios/sincronos")]
    public class RelatoriosSincronosSgpController : ControllerBase
    {
        [HttpPost("faltas-frequencia")]
        public async Task<Guid> RelatorioFaltasFrequencias([FromBody] FiltroRelatorioFrequenciasDto filtro, [FromServices] IRelatorioFrequenciasUseCase relatorioFaltasFrequenciasUseCase)
        {
            var codigoCorrelacao = Guid.NewGuid();
            await relatorioFaltasFrequenciasUseCase.Executar(new FiltroRelatorioDto()
            {
                CodigoCorrelacao = codigoCorrelacao,
                Mensagem = JsonConvert.SerializeObject(filtro)
            });

            return codigoCorrelacao;
        }

        [HttpPost("itinerancias")]
        public async Task<Guid> RelatorioItinerancias([FromBody] FiltroRelatorioItineranciasDto request, [FromServices] IRelatorioItineranciasUseCase relatorioItineranciaUseCase)
        {
            var codigoCorrelacao = Guid.NewGuid();
            await relatorioItineranciaUseCase.Executar(new FiltroRelatorioDto()
            {
                CodigoCorrelacao = codigoCorrelacao,
                Mensagem = JsonConvert.SerializeObject(request)
            });
            
            return codigoCorrelacao;
        }
    }
}
