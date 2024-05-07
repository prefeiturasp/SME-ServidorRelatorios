using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sentry;
using SME.SR.Application;
using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Workers.SGP.Commons.Attributes;
using SME.SR.Workers.SGP.Filters;
using System;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ChaveIntegracaoSrApi]
    [Worker("sme.sr.workers.sgp")]
    public class WorkerSGPController : ControllerBase
    {
        [HttpGet("relatorios/alunos")]
        [Action("relatorios/alunos", typeof(IRelatorioGamesUseCase))]
        public async Task<bool> RelatorioGames([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioGamesUseCase relatorioGamesUseCase)
        {
            await relatorioGamesUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorio/conselhoclasseturma")]
        [Action("relatorio/conselhoclasseturma", typeof(IRelatorioConselhoClasseTurmaUseCase))]
        public async Task<bool> RelatorioConselhoClasseTurma([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioConselhoClasseTurmaUseCase relatorioConselhoClasseTurmaUseCase)
        {
            await relatorioConselhoClasseTurmaUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorio/conselhoclassealuno")]
        [Action("relatorio/conselhoclassealuno", typeof(IRelatorioConselhoClasseAlunoUseCase))]
        public async Task<bool> RelatorioConselhoClasseAluno([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioConselhoClasseAlunoUseCase relatorioConselhoClasseAlunoUseCase)
        {
            await relatorioConselhoClasseAlunoUseCase.Executar(request);
            return true;
        }

        [HttpGet("sr/relatorios/processando")]
        [Action("sr/relatorios/processando", typeof(IMonitorarStatusRelatorioUseCase))]
        public async Task<bool> RelatoriosProcessando([FromQuery] FiltroRelatorioDto request, [FromServices] IMonitorarStatusRelatorioUseCase monitorarStatusRelatorioUseCase)
        {
            await monitorarStatusRelatorioUseCase.Executar(request);
            return true;
        }

        [HttpGet("sr/relatorios/processando/boletim")]
        [Action("sr/relatorios/processando/boletim", typeof(IMonitorarStatusRelatorioUseCase))]
        public async Task<bool> RelatoriosProcessandoBoletim([FromQuery] FiltroRelatorioDto request, [FromServices] IMonitorarStatusRelatorioUseCase monitorarStatusRelatorioUseCase)
        {
            await monitorarStatusRelatorioUseCase.Executar(request);
            return true;
        }

        [HttpGet("sr/relatorios/processando/conselhodeclasse")]
        [Action("sr/relatorios/processando/conselhodeclasse", typeof(IMonitorarStatusRelatorioUseCase))]
        public async Task<bool> RelatoriosProcessandoConselhoClasse([FromQuery] FiltroRelatorioDto request, [FromServices] IMonitorarStatusRelatorioUseCase monitorarStatusRelatorioUseCase)
        {
            await monitorarStatusRelatorioUseCase.Executar(request);
            return true;
        }

        [HttpGet("sr/relatorios/processando/historicoescolar")]
        [Action("sr/relatorios/processando/historicoescolar", typeof(IMonitorarStatusRelatorioUseCase))]
        public async Task<bool> RelatoriosProcessandoHistoricoEscolar([FromQuery] FiltroRelatorioDto request, [FromServices] IMonitorarStatusRelatorioUseCase monitorarStatusRelatorioUseCase)
        {
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
            request.RelatorioEscolaAqui = false;
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

        [HttpGet("relatorios/historicoescolareja")]
        [Action("relatorios/historicoescolareja", typeof(IRelatorioHistoricoEscolarUseCase))]
        public async Task<bool> RelatorioHistoricoEscolarEJA([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioHistoricoEscolarUseCase relatorioHistoricoEscolarUseCase)
        {
            await relatorioHistoricoEscolarUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/pendencias")]
        [Action("relatorios/pendencias", typeof(IRelatorioPendenciasUseCase))]
        public async Task<bool> RelatorioPendencias([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioPendenciasUseCase relatorioPendenciasUseCase)
        {
            await relatorioPendenciasUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/parecerconclusivo")]
        [Action("relatorios/parecerconclusivo", typeof(IRelatorioParecerConclusivoUseCase))]
        public async Task<bool> RelatorioParecerConclusivo([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioParecerConclusivoUseCase relatorioParecerConclusivoUseCase)
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
        public async Task<bool> RelatorioCompensacaoAusencia([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioCompensacaoAusenciaUseCase relatorioCompensacaoAusenciaUseCase)
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
        public async Task<bool> Notificacoes([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioNotificacaoUseCase relatorioUseCase)
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

        [HttpGet("relatorios/conselhoclasseatabimestral")]
        [Action("relatorios/conselhoclasseatabimestral", typeof(IRelatorioConselhoClasseAtaBimestralUseCase))]
        public async Task<bool> RelatorioConselhoClasseAtaBimestralinal([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioConselhoClasseAtaBimestralUseCase relatorioConselhoClasseAtaBimestralUseCase)
        {
            await relatorioConselhoClasseAtaBimestralUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/acompanhamento-frequencia")]
        [Action("relatorios/acompanhamento-frequencia", typeof(IRelatorioAcompanhamentoFrequenciaUseCase))]
        public async Task<bool> AcompanhamentoFrequencia([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioAcompanhamentoFrequenciaUseCase useCase)
        {
            await useCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/acompanhamento-registrospedagogicos")]
        [Action("relatorios/acompanhamento-registrospedagogicos", typeof(IRelatorioAcompanhamentoRegistrosPedagogicosUseCase))]
        public async Task<bool> RelatorioAcompanhamentoRegistrosPedagogicos([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioAcompanhamentoRegistrosPedagogicosUseCase relatorioAcompanhamentoRegistrosPedagogicos)
        {
            await relatorioAcompanhamentoRegistrosPedagogicos.Executar(request);
            return true;
        }

        [HttpGet("relatorios/ocorrencias")]
        [Action("relatorios/ocorrencias", typeof(IRelatorioOcorrenciasUseCase))]
        public async Task<bool> RelatorioOcorrencias([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioOcorrenciasUseCase useCase)
        {
            await useCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/frequencia-global")]
        [Action("relatorios/frequencia-global", typeof(IRelatorioFrequenciaGlobalUseCase))]
        public async Task<bool> RelatorioFrequenciaGlobal([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioFrequenciaGlobalUseCase useCase)
        {
            await useCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/planoaee")]
        [Action("relatorios/planoaee", typeof(IRelatorioPlanoAeeUseCase))]
        public async Task<bool> RelatorioPlanoAee([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioPlanoAeeUseCase useCase)
        {
            await useCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/planosaee")]
        [Action("relatorios/planosaee", typeof(IRelatorioPlanosAeeUseCase))]
        public async Task<bool> RelatorioPlanosAee([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioPlanosAeeUseCase useCase)
        {
            await useCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/encaminhamentosaee")]
        [Action("relatorios/encaminhamentosaee", typeof(IRelatorioEncaminhamentosAeeUseCase))]
        public async Task<bool> RelatorioEncaminhamentosAee([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioEncaminhamentosAeeUseCase useCase)
        {
            await useCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/encaminhamentoaeedetalhado")]
        [Action("relatorios/encaminhamentoaeedetalhado", typeof(IRelatorioEncaminhamentoAeeDetalhadoUseCase))]
        public async Task<bool> RelatorioEncaminhamentoAeeDetalhado([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioEncaminhamentoAeeDetalhadoUseCase useCase)
        {
            await useCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/encaminhamentonaapadetalhado")]
        [Action("relatorios/encaminhamentonaapadetalhado", typeof(IRelatorioEncaminhamentosNaapaDetalhadoUseCase))]
        public async Task<bool> RelatorioEncaminhamentoNaapaDetalhado([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioEncaminhamentosNaapaDetalhadoUseCase detalhadoUseCase)
        {
            await detalhadoUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/encaminhamentosnaapa")]
        [Action("relatorios/encaminhamentosnaapa", typeof(IRelatorioEncaminhamentosNAAPAUseCase))]
        public async Task<bool> RelatorioEncaminhamentosNAAPA([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioEncaminhamentosNAAPAUseCase useCase)
        {
            await useCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/analitico-sondagem")]
        [Action("relatorios/analitico-sondagem", typeof(IRelatorioAnaliticoSondagemUseCase))]
        public async Task<bool> RelatorioAnalicoDaSondagem([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioAnaliticoSondagemUseCase useCase)
        {
            await useCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/listagem-itinerancias")]
        [Action("relatorios/listagem-itinerancias", typeof(IRelatorioListagemItineranciasUseCase))]
        public async Task<bool> ListarItinerancias([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioListagemItineranciasUseCase relatorioUseCase)
        {
            await relatorioUseCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/controle-frequencia-mensal")]
        [Action("relatorios/controle-frequencia-mensal", typeof(IRelatorioFrequenciaControleMensalUseCase))]
        public async Task<bool> FrequenciaControleMensal([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioFrequenciaControleMensalUseCase useCase)
        {
            await useCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/listagem-ocorrencias")]
        [Action("relatorios/listagem-ocorrencias", typeof(IRelatorioListagemOcorrenciasUseCase))]
        public async Task<bool> ListagemOcorrencias([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioListagemOcorrenciasUseCase useCase)
        {
            await useCase.Executar(request);
            return true;
        }

        [HttpGet("relatorios/mapeamentosestudantes")]
        [Action("relatorios/mapeamentosestudantes", typeof(IRelatorioMapeamentosEstudantesUseCase))]
        public async Task<bool> RelatorioMapeamentosEstudantes([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioMapeamentosEstudantesUseCase useCase)
        {
            var filtroRelatorioDto = new FiltroRelatorioDto()
            {
                Action = "relatorios/mapeamentosestudantes",
                UsuarioLogadoRF = "6769195",
                CodigoCorrelacao = new Guid("AF3A5649-E805-4CB3-B73E-5191C445DE19"),
            };

            var relatorio = new FiltroRelatorioMapeamentoEstudantesDto()
            {
                AlunoCodigo = "",
                AnoLetivo = 2024,
                Modalidade = Modalidade.Fundamental,
                UeCodigo = "094617", 
                TurmasCodigo = new string[] { "2659881", "2660389" }
            };

            var mensagem = JsonConvert.SerializeObject(relatorio, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            filtroRelatorioDto.Mensagem = mensagem;

            await useCase.Executar(filtroRelatorioDto);//request
            return true;
        }

        #region App Escola Aqui
        [HttpGet("relatorios/acompanhamento-aprendizagem-escolaaqui")]
        [Action("relatorios/acompanhamento-aprendizagem-escolaaqui", typeof(IRelatorioAcompanhamentoAprendizagemUseCase))]
        public async Task<bool> AcompanhamentoAprendizagemEscolaAqui([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioAcompanhamentoAprendizagemUseCase relatorioUseCase)
        {
            request.RelatorioEscolaAqui = true;
            await relatorioUseCase.Executar(request);
            return true;
        }
        [HttpGet("relatorios/boletimescolardetalhadoescolaaqui")]
        [Action("relatorios/boletimescolardetalhadoescolaaqui", typeof(IRelatorioBoletimEscolarDetalhadoUseCase))]
        public async Task<bool> RelatorioBoletimEscolarDetalhadoEscolaAqui([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioBoletimEscolarDetalhadoUseCase relatorioBoletimEscolarDetalhadoUseCase)
        {
            request.RelatorioEscolaAqui = true;
            await relatorioBoletimEscolarDetalhadoUseCase.Executar(request);
            return true;
        }
        #endregion App Escola Aqui
        
        [HttpGet("relatorios/plano-anual")]
        [Action("relatorios/plano-anual", typeof(IRelatorioPlanoAnualUseCase))]
        public async Task<bool> RelatorioPlanoAnual([FromQuery] FiltroRelatorioDto request, [FromServices] IRelatorioPlanoAnualUseCase relatorioPlanoAnualUseCase)
        {
            await relatorioPlanoAnualUseCase.Executar(request);
            return true;
        }
    }
}
