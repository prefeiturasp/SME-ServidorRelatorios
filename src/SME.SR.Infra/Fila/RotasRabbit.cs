namespace SME.SR.Infra
{
    //TODO: USAR VARIAVEIS DE AMBIENT?
    public static class RotasRabbit
    {
        public static string ExchangeListenerWorkerRelatorios => "sme.sr.workers.relatorios";


        public static string FilaWorkerRelatorios => "sme.sr.workers.sgp";
        public static string FilaClientsSgp => "sme.sr.clients.sgp";
        
        public static string RotaRelatoriosProntosSgp => "relatorios.prontos";
        public static string RotaRelatoriosProcessando => "relatorios.processando";
        
    }
}
