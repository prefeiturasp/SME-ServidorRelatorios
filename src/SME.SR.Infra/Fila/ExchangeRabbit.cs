namespace SME.SR.Infra
{
    public static class ExchangeRabbit
    {
        public static string ExchangeListenerWorkerRelatorios => "sme.sr.workers.relatorios";
        public static string ExchangeListenerWorkerRelatoriosDeadletter => "sme.sr.workers.relatorios.deadletter";
        public static string ExchangeSgp => "sme.sgp.workers";
        public static string ExchangeSgpDeadLetter => "sme.sgp.workers.deadletter";
    }
}
