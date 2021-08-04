namespace SME.SR.Infra
{
    //TODO: USAR VARIAVEIS DE AMBIENT?
    //TODO: Camada compartilhada entre projetos, virar um pacote NPM?
    public static class RotasRabbit
    {
        //public static string FilaWorkerRelatorios => "sme.sr.workers.sgp";
        //public static string FilaSgp => "sme.sgp.clients";
        public const string RotaRelatoriosSolicitados = "relatorios.solicitados";
        public const string RotaRelatoriosProcessando = "relatorios.processando";


        public const string RotaRelatoriosProntosSgp = "relatorios.prontos";
        public const string RotaRelatorioComErro = "relatorios.erro";
        public const string RotaRelatorioCorrelacaoCopiar = "relatorios.correlacao.copiar";
        public const string RotaRelatorioCorrelacaoInserir = "relatorios.correlacao.inserir";


        public const string RotaRelatoriosSolicitadosBoletim = "relatorios.solicitados.boletim";
        public const string RotaRelatoriosProcessandoBoletim = "relatorios.processando.boletim";

        public const string RotaRelatoriosSolicitadosBoletimDetalhado = "relatorios.solicitados.boletimdetalhado";

        public const string RotaRelatoriosSolicitadosPlanoDeAula = "relatorios.solicitados.planodeaula";
        public const string RotaRelatoriosSolicitadosRegistroIndividual = "relatorios.solicitados.registroindividual";
        public const string RotaRelatoriosSolicitadosConselhoDeClasse = "relatorios.solicitados.conselhodeclasse";
        public const string RotaRelatoriosProcessandoConselhoDeClasse = "relatorios.processando.conselhodeclasse";
        public const string RotaRelatoriosSolicitadosRelatorioAcompanhamentoAprendizagem = "relatorios.solicitados.relatorioacompanhamentoaprendizagem";
        public const string RotaRelatoriosSolicitadosCalendarioEscolar = "relatorios.solicitados.calendárioescolar";
        public const string RotaRelatoriosSolicitadosRegistroItinerancia = "relatorios.solicitados.registroitinerancia";
        public const string RotaRelatoriosSolicitadosPendencias = "relatorios.solicitados.pendencias";
        public const string RotaRelatoriosSolicitadosAcompanhamentoFechamento = "relatorios.solicitados.acompanhamentofechamento";
        public const string RotaRelatoriosSolicitadosParecerConclusivo = "relatorios.solicitados.parecerconclusivo";
        public const string RotaRelatoriosSolicitadosNotasConceitosFinais = "relatorios.solicitados.notasconceitosfinais";
        public const string RotaRelatoriosSolicitadosRelatorioAlteraçãoNotas = "relatorios.solicitados.relatorioalteracaonotas";
        public const string RotaRelatoriosSolicitadosPapResumos = "relatorios.solicitados.papresumos";
        public const string RotaRelatoriosSolicitadosPapGraficos = "relatorios.solicitados.papgraficos";
        public const string RotaRelatoriosSolicitadosPapRelatorioSemestral = "relatorios.solicitados.paprelatoriosemestral";
        public const string RotaRelatoriosSolicitadosFrequencia = "relatorios.solicitados.frequencia";
        public const string RotaRelatoriosSolicitadosCompensacaoAusencia = "relatorios.solicitados.compensacaoausencia";
        public const string RotaRelatoriosSolicitadosControleGrade = "relatorios.solicitados.controlegrade";
        public const string RotaRelatoriosSolicitadosControlePlanejamentoDiario = "relatorios.solicitados.controleplanejamentodiario";
        public const string RotaRelatoriosSolicitadosDevolutivas = "relatorios.solicitados.devolutivas";
        public const string RotaRelatoriosSolicitadosUsuarios = "relatorios.solicitados.usuarios";
        public const string RotaRelatoriosSolicitadosAtribuicoes = "relatorios.solicitados.atribuicoes";
        public const string RotaRelatoriosSolicitadosNotificacoes = "relatorios.solicitados.notificacoes";
        public const string RotaRelatoriosSolicitadosEscolaAquiLeitura = "relatorios.solicitados.escolaaquileitura";
        public const string RotaRelatoriosSolicitadosEscolaAquiAdesao = "relatorios.solicitados.escolaaquiadesao";
        public const string RotaRelatoriosSolicitadosAtaFinalResultados = "relatorios.solicitados.atafinalresultados";
        public const string RotaRelatoriosSolicitadosHistoricoEscolar = "relatorios.solicitados.historicoescolar";
        public const string RotaRelatoriosProcessandoHistoricoEscolar = "relatorios.processando.historicoescolar";
    }
}
