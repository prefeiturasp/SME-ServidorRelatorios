namespace SME.SR.Infra
{
    //TODO: USAR VARIAVEIS DE AMBIENT?
    //TODO: Camada compartilhada entre projetos, virar um pacote NPM?
    public static class RotasRabbit
    {
        public static string ExchangeListenerWorkerRelatorios => "sme.sr.workers.relatorios";
        public static string ExchangeListenerWorkerRelatoriosDeadletter => "sme.sr.workers.relatorios.deadletter";
        public static string ExchangeSgp => "sme.sgp.workers";

        public static string FilaWorkerRelatorios => "sme.sr.workers.sgp";
        public static string FilaSgp => "sme.sgp.clients";
        public static string RotaRelatoriosSolicitados => "relatorios.solicitados";
        public static string RotaRelatoriosProntosSgp => "relatorios.prontos";
        public static string RotaRelatoriosProcessando => "relatorios.processando";

        public static string RotaRelatorioComErro => "relatorios.erro";
        public static string RotaRelatorioCorrelacaoCopiar => "relatorios.correlacao.copiar";
        public static string RotaRelatorioCorrelacaoInserir => "relatorios.correlacao.inserir";       
    }
}
