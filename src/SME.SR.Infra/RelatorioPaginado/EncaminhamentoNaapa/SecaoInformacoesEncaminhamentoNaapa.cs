namespace SME.SR.Infra
{
    public class SecaoInformacoesEncaminhamentoNaapa : SecaoRelatorioEncaminhamentoNaapa
    {
        private const int TOTAL_LINHAS = 2;
        public SecaoInformacoesEncaminhamentoNaapa(SecaoQuestoesEncaminhamentoNAAPADetalhadoDto secaoQuestao)
        {
            DataDeEntradaQueixa = ObterValor(secaoQuestao, NomeComponentesEncaminhamentoNaapa.DATA_ENTRADA_QUEIXA);
            Prioridade = ObterValor(secaoQuestao, NomeComponentesEncaminhamentoNaapa.PRIORIDADE);
            PortaDeEntrada = ObterValor(secaoQuestao, NomeComponentesEncaminhamentoNaapa.PORTA_ENTRADA);
            NIS = ObterValor(secaoQuestao, NomeComponentesEncaminhamentoNaapa.NIS);
            CNS = ObterValor(secaoQuestao, NomeComponentesEncaminhamentoNaapa.CNS);
        }

        public string DataDeEntradaQueixa { get; set; } 
        public string Prioridade { get; set; }
        public string PortaDeEntrada { get; set; }
        public string NIS { get; set; }
        public string CNS { get; set; }

        public override int ObterLinhasDeQuebra()
        {
            return TOTAL_LINHAS;
        }
    }
}
