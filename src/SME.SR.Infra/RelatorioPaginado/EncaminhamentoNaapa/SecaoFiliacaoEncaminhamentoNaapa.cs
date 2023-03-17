namespace SME.SR.Infra
{
    public class SecaoFiliacaoEncaminhamentoNaapa : SecaoRelatorioEncaminhamentoNaapa
    {
        private const int COLUNA_FILIACAO = 50;
        private const int COLUNA_GRUPO = 40;
        private const int COLUNA_UBS = 55;
        private const int TOTAL_LINHAS = 3;

        public SecaoFiliacaoEncaminhamentoNaapa(SecaoQuestoesEncaminhamentoNAAPADetalhadoDto secaoQuestao)
        {
            Filiacao = ObterValor(secaoQuestao, NomeComponentesEncaminhamentoNaapa.FILIACAO);
            Genero = ObterValor(secaoQuestao, NomeComponentesEncaminhamentoNaapa.GENERO);
            GrupoEtnico = ObterValor(secaoQuestao, NomeComponentesEncaminhamentoNaapa.GRUPO_ETNICO);
            EstudanteMigrante = ObterValor(secaoQuestao, NomeComponentesEncaminhamentoNaapa.ESTUDANTE_MIGRANTE);
            ResponsavelMigrante = ObterValor(secaoQuestao, NomeComponentesEncaminhamentoNaapa.RESPONSAVEL_MIGRANTE);
            UBS = ObterValor(secaoQuestao, NomeComponentesEncaminhamentoNaapa.UBS);
            CRAS = ObterValor(secaoQuestao, NomeComponentesEncaminhamentoNaapa.CRAS);
        }

        public string Filiacao { get; set; }
        public string Genero { get; set; }
        public string GrupoEtnico { get; set; }
        public string EstudanteMigrante { get; set; }
        public string ResponsavelMigrante { get; set; }
        public string UBS { get; set; }
        public string CRAS { get; set; }

        public override int ObterLinhasDeQuebra()
        {
            return TOTAL_LINHAS + ObterTotalQuebraPorColuna();
        }

        private int ObterTotalQuebraPorColuna()
        {
            var linhas = 0;

            if (Filiacao.Length > COLUNA_FILIACAO || GrupoEtnico.Length > COLUNA_GRUPO)
                linhas += 1;

            if (UBS.Length > COLUNA_UBS)
                linhas += 1;

            return linhas;
        }
    }
}
