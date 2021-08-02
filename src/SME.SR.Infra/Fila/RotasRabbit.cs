namespace SME.SR.Infra
{
    //TODO: USAR VARIAVEIS DE AMBIENT?
    //TODO: Camada compartilhada entre projetos, virar um pacote NPM?
    public static class RotasRabbit
    {
        //public static string FilaWorkerRelatorios => "sme.sr.workers.sgp";
        //public static string FilaSgp => "sme.sgp.clients";
        public const string RotaRelatoriosSolicitados = "relatorios.solicitados";
        public const string RotaRelatoriosProntosSgp = "relatorios.prontos";
        public const string RotaRelatoriosProcessando = "relatorios.processando";

        public const string RotaRelatorioComErro = "relatorios.erro";
        public const string RotaRelatorioCorrelacaoCopiar = "relatorios.correlacao.copiar";
        public const string RotaRelatorioCorrelacaoInserir = "relatorios.correlacao.inserir";


        public const string RotaRelatoriosSolicitadosBoletim = "relatorios.solicitados.boletim";
        public const string RotaRelatoriosSolicitadosBoletimDetalhado = "relatorios.solicitados.boletimdetalhado";
    }
}
