namespace SME.SR.Infra
{
    public class SecaoItensItineranciaEncaminhamentoNaapa : SecaoRelatorioEncaminhamentoNaapa
    {
        private const int COLUNA_TIPO = 52;
        private const int COLUNA_PROCEDIMENTO = 52;
        private const int TOTAL_LINHAS = 2;

        public SecaoItensItineranciaEncaminhamentoNaapa(QuestaoEncaminhamentoNAAPADetalhadoDto QuestaoData, SecaoQuestoesEncaminhamentoNAAPADetalhadoDto secaoQuestao)
        {
            Titulo = ObterValorFormatado(QuestaoData).ToUpper();
            TipoAtendimento = ObterValor(secaoQuestao, NomeComponentesEncaminhamentoNaapa.TIPO_DO_ATENDIMENTO);
            ProcedimentoDeTrabalho = ObterValor(secaoQuestao, NomeComponentesEncaminhamentoNaapa.PROCEDIMENTO_DE_TRABALHO);
        }

        public string Titulo { get; set; }
        public string TipoAtendimento { get; set; }
        public string ProcedimentoDeTrabalho { get; set; }

        public override int ObterLinhasDeQuebra()
        {
            return TOTAL_LINHAS + ObterTotalQuebraPorColuna(); 
        }

        private int ObterTotalQuebraPorColuna()
        {
            var linhas = 0;

            if (TipoAtendimento.Length > COLUNA_TIPO || ProcedimentoDeTrabalho.Length > COLUNA_PROCEDIMENTO)
                linhas += 1;

            return linhas;
        }
    }
}
