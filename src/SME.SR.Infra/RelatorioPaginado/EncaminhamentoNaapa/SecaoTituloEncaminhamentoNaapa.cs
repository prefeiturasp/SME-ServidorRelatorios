namespace SME.SR.Infra
{
    public class SecaoTituloEncaminhamentoNaapa : SecaoRelatorioEncaminhamentoNaapa
    {
        private const int TOTAL_LINHA_TITULO = 1;
        private const int TOTAL_LINHA_SUBTITULO = 2;
        public SecaoTituloEncaminhamentoNaapa(string titulo) : this(titulo, string.Empty)
        {
        }

        public SecaoTituloEncaminhamentoNaapa(string titulo, string subTitulo, bool primeiraLinha = false)
        {
            Titulo = titulo.ToUpper();
            SubTitulo = subTitulo.ToUpper();
            PrimeiraLinha = primeiraLinha;
        }

        public string Titulo { get; set; }
        public string SubTitulo { get; set; }
        public bool PrimeiraLinha { get; set; }

        public override int ObterLinhasDeQuebra()
        {
            return string.IsNullOrEmpty(SubTitulo) ? TOTAL_LINHA_TITULO : TOTAL_LINHA_SUBTITULO;
        }
    }
}
