using Microsoft.AspNetCore.Mvc;
using Sentry;
using SME.SR.Application;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using SME.SR.Workers.SGP.Commons.Attributes;
using System;
using System.Threading.Tasks;


namespace SME.SR.Workers.SGP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Worker("sme.sr.workers.sgp")]
    public class WorkerSGPController : ControllerBase
    {

        public WorkerSGPController()
        {

        }
        [HttpGet("relatorios/alunos")]
        [Action("relatorios/alunos", typeof(IRelatorioGamesUseCase))]
        public async Task<bool> RelatorioGames([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioGamesUseCase relatorioGamesUseCase)
        {
            await relatorioGamesUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorio/conselhoclasseturma")]
        [Action("relatorio/conselhoclasseturma", typeof(IRelatorioConselhoClasseTurmaUseCase))]
        public async Task<bool> RelatorioConselhoClasseTurma([FromQuery] FiltroRelatorioDto request, IRelatorioConselhoClasseTurmaUseCase relatorioConselhoClasseTurmaUseCase)
        {
            await relatorioConselhoClasseTurmaUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorio/conselhoclassealuno")]
        [Action("relatorio/conselhoclassealuno", typeof(IRelatorioConselhoClasseAlunoUseCase))]
        public async Task<bool> RelatorioConselhoClasseAluno([FromQuery] FiltroRelatorioDto request, IRelatorioConselhoClasseAlunoUseCase relatorioConselhoClasseAlunoUseCase)
        {
            SentrySdk.CaptureMessage("4 - relatorio/conselhoclassealuno");
            await relatorioConselhoClasseAlunoUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/processando")]
        [Action("relatorios/processando", typeof(IMonitorarStatusRelatorioUseCase))]
        public async Task<bool> RelatoriosProcessando([FromQuery] FiltroRelatorioDto request, [FromServices] IMonitorarStatusRelatorioUseCase monitorarStatusRelatorioUseCase)
        {
            SentrySdk.CaptureMessage("7 - relatorios/processando");
            await monitorarStatusRelatorioUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/boletimescolar")]
        [Action("relatorios/boletimescolar", typeof(IRelatorioBoletimEscolarUseCase))]
        public async Task<bool> RelatorioBoletimEscolar([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioBoletimEscolarUseCase relatorioBoletimEscolarUseCase)
        {
            await relatorioBoletimEscolarUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/boletimescolardetalhado")]
        [Action("relatorios/boletimescolardetalhado", typeof(IRelatorioBoletimEscolarDetalhadoUseCase))]
        public async Task<bool> RelatorioBoletimEscolarDetalhado([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioBoletimEscolarDetalhadoUseCase relatorioBoletimEscolarDetalhadoUseCase)
        {
            await relatorioBoletimEscolarDetalhadoUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/conselhoclasseatafinal")]
        [Action("relatorios/conselhoclasseatafinal", typeof(IRelatorioConselhoClasseAtaFinalUseCase))]
        public async Task<bool> RelatorioConselhoClasseAtaFinal([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioConselhoClasseAtaFinalUseCase relatorioConselhoClasseAtaFinalUseCase)
        {
            await relatorioConselhoClasseAtaFinalUseCase.Executar(request);
            return true;
        }

        [HttpPost("relatorios/frequencia")]
        [Action("relatorios/frequencia", typeof(IRelatorioFrequenciasUseCase))]
        public async Task<bool> RelatorioFrequencias([FromBody] FiltroRelatorioDto request, [FromServices] IRelatorioFrequenciasUseCase relatorioFaltasFrequenciasUseCase)
        {
            await relatorioFaltasFrequenciasUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/historicoescolarfundamental")]
        [Action("relatorios/historicoescolarfundamental", typeof(IRelatorioHistoricoEscolarUseCase))]
        public async Task<bool> RelatorioHistoricoEscolar([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioHistoricoEscolarUseCase relatorioHistoricoEscolarUseCase)
        {
            await relatorioHistoricoEscolarUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/fechamentopendencias")]
        [Action("relatorios/fechamentopendencias", typeof(IRelatorioFechamentoPendenciasUseCase))]
        public async Task<bool> RelatorioFechamentoPendencias([FromQuery] FiltroRelatorioDto request, [FromServices]IRelatorioFechamentoPendenciasUseCase relatorioFechamentoPendenciasUseCase)
        {
            await relatorioFechamentoPendenciasUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/parecerconclusivo")]
        [Action("relatorios/parecerconclusivo", typeof(IRelatorioParecerConclusivoUseCase))]
        public async Task<bool> RelatorioParecerConclusivo([FromQuery] FiltroRelatorioDto request, [FromServices]IRelatorioParecerConclusivoUseCase relatorioParecerConclusivoUseCase)
        {
            await relatorioParecerConclusivoUseCase.Executar(request);
            return true;
        }
        
        [HttpGet("relatorios/recuperacaoparalela")]
        [Action("relatorios/recuperacaoparalela", typeof(IRelatorioRecuperacaoParalelaUseCase))]
        public async Task<bool> RelatorioRecuperacaoParalela([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioRecuperacaoParalelaUseCase relatorioRecuperacaoParalelaUseCase)
        {
            await relatorioRecuperacaoParalelaUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/notasconceitosfinais")]
        [Action("relatorios/notasconceitosfinais", typeof(IRelatorioNotasEConceitosFinaisUseCase))]
        public async Task<bool> RelatorioNotasEConceitosFinais([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioNotasEConceitosFinaisUseCase relatorioNotasEConceitosFinaisUseCase)
        {
            await relatorioNotasEConceitosFinaisUseCase.Executar(request);
            return true;
        }
        [HttpGet("relatorios/compensacaoausencia")]
        [Action("relatorios/compensacaoausencia", typeof(IRelatorioCompensacaoAusenciaUseCase))]
        public async Task<bool> RelatorioCompensacaoAusencia([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioCompensacaoAusenciaUseCase  relatorioCompensacaoAusenciaUseCase)
        {
            await relatorioCompensacaoAusenciaUseCase.Executar(request);
            return true;
        }
        [HttpGet("relatorios/impressaocalendario")]
        [Action("relatorios/impressaocalendario", typeof(IRelatorioImpressaoCalendarioUseCase))]
        public async Task<bool> RelatorioImpressaoCalendario([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioImpressaoCalendarioUseCase relatorioImpressaoCalendarioUseCase)
        {
            await relatorioImpressaoCalendarioUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/resumopap")]
        [Action("relatorios/resumopap", typeof(IRelatorioResumoPAPUseCase))]
        public async Task<bool> RelatorioResumoPAP([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioResumoPAPUseCase relatorioResumoPAPUseCase)
        {
            await relatorioResumoPAPUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/graficopap")]
        [Action("relatorios/graficopap", typeof(IRelatorioGraficoPAPUseCase))]
        public async Task<bool> RelatorioGraficoPAP([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioGraficoPAPUseCase relatorioGraficoPAPUseCase)
        {
            await relatorioGraficoPAPUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/planoaula")]
        [Action("relatorios/planoaula", typeof(IRelatorioPlanoAulaUseCase))]
        public async Task<bool> RelatorioPlanoAula([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioPlanoAulaUseCase relatorioPlanoAulaUseCase)
        {
            await relatorioPlanoAulaUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/controle-grade")]
        [Action("relatorios/controle-grade", typeof(IRelatorioControleGradeUseCase))]
        public async Task<bool> ControleGrade([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioControleGradeUseCase relatorioUseCase)
        {
            await relatorioUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/usuarios")]
        [Action("relatorios/usuarios", typeof(IRelatorioUsuariosUseCase))]
        public async Task<bool> Usuarios([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioUsuariosUseCase relatorioUseCase)
        {
            await relatorioUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/notificacoes")]
        [Action("relatorios/notificacoes", typeof(IRelatorioNotificacaoUseCase))]
        public async Task<bool> ControleGrade([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioNotificacaoUseCase relatorioUseCase)
        {
            await relatorioUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/atribuicoes-cj")]
        [Action("relatorios/atribuicoes-cj", typeof(IRelatorioAtribuicaoCJUseCase))]
        public async Task<bool> AtribuicoesCJ([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioAtribuicaoCJUseCase relatorioUseCase)
        {
            await relatorioUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/alteracao-notas")]
        [Action("relatorios/alteracao-notas", typeof(IRelatorioAlteracaoNotasUseCase))]
        public async Task<bool> AlteracaoNotasBimestre([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioAlteracaoNotasUseCase relatorioUseCase)
        {
            await relatorioUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/ae/adesao")]
        [Action("relatorios/ae/adesao", typeof(IRelatorioAdesaoAppUseCase))]
        public async Task<bool> AdesaoApp([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioAdesaoAppUseCase relatorioUseCase)
        {
            await relatorioUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/dados-leitura")]
        [Action("relatorios/dados-leitura", typeof(IRelatorioLeituraComunicadosUseCase))]
        public async Task<bool> LeituraComunicados([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioLeituraComunicadosUseCase relatorioUseCase)
        {
            await relatorioUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/planejamento-diario")]
        [Action("relatorios/planejamento-diario", typeof(IRelatorioPlanejamentoDiarioUseCase))]
        public async Task<bool> PlanejamentoDiario([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioPlanejamentoDiarioUseCase relatorioUseCase)
        {
            await relatorioUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/devolutivas")]
        [Action("relatorios/devolutivas", typeof(IRelatorioDevolutivasUseCase))]
        public async Task<bool> Devolutivas([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioDevolutivasUseCase relatorioUseCase)
        {
            await relatorioUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/itinerancias")]
        [Action("relatorios/itinerancias", typeof(IRelatorioItineranciasUseCase))]
        public async Task<bool> Itinerancias([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioItineranciasUseCase relatorioUseCase)
        {
            await relatorioUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/registro-individual")]
        [Action("relatorios/registro-individual", typeof(IRelatorioRegistroIndividualUseCase))]
        public async Task<bool> RegistroIndividual([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioRegistroIndividualUseCase relatorioUseCase)
        {
            await relatorioUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/acompanhamento-aprendizagem")]
        [Action("relatorios/acompanhamento-aprendizagem", typeof(IRelatorioAcompanhamentoAprendizagemUseCase))]
        public async Task<bool> AcompanhamentoAprendizagem([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioAcompanhamentoAprendizagemUseCase relatorioUseCase)
        {
            await relatorioUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/acompanhamento-fechamento")]
        [Action("relatorios/acompanhamento-fechamento", typeof(IRelatorioAcompanhamentoFechamentoUseCase))]
        public async Task<bool> AcompanhamentoFechamento([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioAcompanhamentoFechamentoUseCase relatorioUseCase)
        {
            await relatorioUseCase.Executar(request);
            return true;
        }
    }
}
