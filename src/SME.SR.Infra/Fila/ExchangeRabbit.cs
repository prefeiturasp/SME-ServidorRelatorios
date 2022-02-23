namespace SME.SR.Infra
{
    public static class ExchangeRabbit
    {
        public static string WorkerRelatorios => "sme.sr.workers.relatorios";
        public static string WorkerRelatoriosDeadletter => "sme.sr.workers.relatorios.deadletter";
        public static string Sgp => "sme.sgp.workers";
        public static string SgpDeadLetter => "sme.sgp.workers.deadletter";
        public static string SgpLogs => "EnterpriseApplicationLog";
    }
}
