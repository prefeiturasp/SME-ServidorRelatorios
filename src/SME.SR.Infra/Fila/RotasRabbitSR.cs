﻿using System.Collections.Generic;

namespace SME.SR.Infra
{
    public static class RotasRabbitSR
    {
        public const string RotaRelatoriosSolicitados = "sr.relatorios.solicitados";
        public const string RotaRelatoriosProcessando = "sr.relatorios.processando";
        public const string RotaRelatoriosSolicitadosBoletim = "sr.relatorios.solicitados.boletim";
        public const string RotaRelatoriosProcessandoBoletim = "sr.relatorios.processando.boletim";

        public const string RotaRelatoriosSolicitadosBoletimTurma = "sr.relatorios.solicitados.boletim.turma";
        public const string RotaRelatoriosProcessandoBoletimTurma = "sr.relatorios.processando.boletim.turma";

        public const string RotaRelatoriosSolicitadosBoletimDetalhado = "sr.relatorios.solicitados.boletimdetalhado";
        public const string RotaRelatoriosSolicitadosBoletimDetalhadoEscolaAqui = "sr.relatorios.app.solicitados.boletimdetalhado";

        public const string RotaRelatoriosSolicitadosPlanoDeAula = "sr.relatorios.solicitados.planodeaula";

        public const string RotaRelatoriosSolicitadosRegistroIndividual = "sr.relatorios.solicitados.registroindividual";

        public const string RotaRelatoriosSolicitadosConselhoDeClasse = "sr.relatorios.solicitados.conselhodeclasse";
        public const string RotaRelatoriosProcessandoConselhoDeClasse = "sr.relatorios.processando.conselhodeclasse";

        public const string RotaRelatoriosSolicitadosRelatorioAcompanhamentoAprendizagem = "sr.relatorios.solicitados.relatorioacompanhamentoaprendizagem";
        public const string RotaRelatoriosSolicitadosRaaEscolaAqui = "sr.relatorios.app.solicitados.raaescolaaqui";

        public const string RotaRelatoriosSolicitadosCalendarioEscolar = "sr.relatorios.solicitados.calendárioescolar";

        public const string RotaRelatoriosSolicitadosRegistroItinerancia = "sr.relatorios.solicitados.registroitinerancia";

        public const string RotaRelatoriosSolicitadosPendencias = "sr.relatorios.solicitados.pendencias";

        public const string RotaRelatoriosSolicitadosAcompanhamentoFechamento = "sr.relatorios.solicitados.acompanhamentofechamento";

        public const string RotaRelatoriosSolicitadosParecerConclusivo = "sr.relatorios.solicitados.parecerconclusivo";

        public const string RotaRelatoriosSolicitadosNotasConceitosFinais = "sr.relatorios.solicitados.notasconceitosfinais";

        public const string RotaRelatoriosSolicitadosRelatorioAlteraçãoNotas = "sr.relatorios.solicitados.relatorioalteracaonotas";

        public const string RotaRelatoriosSolicitadosPapResumos = "sr.relatorios.solicitados.papresumos";

        public const string RotaRelatoriosSolicitadosPapGraficos = "sr.relatorios.solicitados.papgraficos";

        public const string RotaRelatoriosSolicitadosPapRelatorioSemestral = "sr.relatorios.solicitados.paprelatoriosemestral";

        public const string RotaRelatoriosSolicitadosFrequencia = "sr.relatorios.solicitados.frequencia";

        public const string RotaRelatoriosSolicitadosFrequenciaMensal = "sr.relatorios.solicitados.frequenciamensal";

        public const string RotaRelatoriosSolicitadosCompensacaoAusencia = "sr.relatorios.solicitados.compensacaoausencia";

        public const string RotaRelatoriosSolicitadosControleGrade = "sr.relatorios.solicitados.controlegrade";

        public const string RotaRelatoriosSolicitadosControlePlanejamentoDiario = "sr.relatorios.solicitados.controleplanejamentodiario";

        public const string RotaRelatoriosSolicitadosDevolutivas = "sr.relatorios.solicitados.devolutivas";

        public const string RotaRelatoriosSolicitadosUsuarios = "sr.relatorios.solicitados.usuarios";

        public const string RotaRelatoriosSolicitadosAtribuicoes = "sr.relatorios.solicitados.atribuicoes";

        public const string RotaRelatoriosSolicitadosNotificacoes = "sr.relatorios.solicitados.notificacoes";

        public const string RotaRelatoriosSolicitadosEscolaAquiLeitura = "sr.relatorios.solicitados.escolaaquileitura";

        public const string RotaRelatoriosSolicitadosEscolaAquiAdesao = "sr.relatorios.solicitados.escolaaquiadesao";

        public const string RotaRelatoriosSolicitadosAtaFinalResultados = "sr.relatorios.solicitados.atafinalresultados";

        public const string RotaRelatoriosSolicitadosHistoricoEscolar = "sr.relatorios.solicitados.historicoescolar";
        public const string RotaRelatoriosProcessandoHistoricoEscolar = "sr.relatorios.processando.historicoescolar";

        public const string RotaRelatoriosSolicitadosAtaBimestralResultados = "sr.relatorios.solicitados.atabimestralresultados";
        public const string RotaRelatoriosSolicitadosAcompanhamentoFrequencia = "sr.relatorios.solicitados.acompanhamentofrequencia";
        public const string RotaRelatoriosSolicitadosAcompanhamentoRegistrosPedagogicos = "sr.relatorios.solicitados.acompanhamentoregistrospedagogicos";
        public const string RotaRelatoriosSolicitadosOcorrencias = "sr.relatorios.solicitados.ocorrencias";
        public const string RotaRelatoriosSolicitatosPlanoAee = "sr.relatorios.solicitados.planoaee";
        public const string RotaRelatoriosSolicitatosPlanosAee = "sr.relatorios.solicitados.planosaee";
        public const string RotaRelatoriosSolicitadosEncaminhamentoAee = "sr.relatorios.solicitados.encaminhamentoaee";
        public const string RotaRelatoriosSolicitadosEncaminhamentoAeeDetalhado = "sr.relatorios.solicitados.RelatorioEncaminhamentoAeeDetalhado";
        public const string RotaRelatoriosSolicitadosEncaminhamentoNaapaDetalhado = "sr.relatorios.solicitados.encaminhamentonaapa.detalhado";
        public const string RotaRelatoriosSolicitadosEncaminhamentoNAAPA = "sr.relatorios.solicitados.encaminhamentonaapa";
        public const string RotaRelatoriosSolicitadosAnaliticoSondagem = "sr.relatorios.solicitados.sondagem.analitico";
        public const string RotaRelatoriosSolicitadosAnaliticoSondagemTodasDresUes = "sr.relatorios.solicitados.sondagem.analitico.todas.dres.ues";
        public const string RotaRelatoriosSolicitadosListagemRegistrosItinerancia = "sr.relatorios.solicitados.listagem.registrositinerancia";
        public const string RotaRelatoriosSolicitadosControleFrequenciaMensal = "sr.relatorios.solicitados.controle.frequencia.mensal";
        public const string RotaRelatoriosSolicitadosListagemOcorrencias = "sr.relatorios.solicitados.listagem.ocorrencias";
        public const string RotaRelatoriosSolicitatosPlanoAnual = "sr.relatorios.solicitados.planoanual";
        public const string RotaRelatoriosSolicitadosMapeamentoEstudante = "sr.relatorios.solicitados.mapeamentoestudante";
        public const string RotaRelatoriosSolicitadosBuscaAtiva = "sr.relatorios.solicitados.buscaativa";
        public const string RotaRelatoriosSolicitadosProdutividadeFrequencia = "sr.relatorios.solicitados.produtividade.frequencia";
    }

    public static class ArgumentosRabbitSR
    {
        private const string ARGUMENTO_X_CONSUMER_TIMEOUT = "x-consumer-timeout";
        private static readonly Dictionary<string, Dictionary<string, object>> configuracoes = new Dictionary<string, Dictionary<string, object>>();

        static ArgumentosRabbitSR()
        {
            /*configuracoes[RotasRabbitSR.RotaRelatoriosSolicitadosFrequenciaMensal] = 
                new Dictionary<string, object>
                {
                    { ARGUMENTO_X_CONSUMER_TIMEOUT, 4 * 3600000 }
                }; */
        }

        public static Dictionary<string, object> ObterConfiguracao(string nomeFila)
            => configuracoes.TryGetValue(nomeFila, out var configuracao) ? configuracao : null;
    }
}
