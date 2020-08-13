namespace SME.SR.Infra
{
    public class RelatorioRecuperacaoParalelaRetornoQueryDto
    {
        public RelatorioRecuperacaoParalelaRetornoQueryDto(string alunoCodigo, string turmaCodigo, string turmaNome, int anoLetivo, string secaoNome, string secaoValor)
        {

            AlunoCodigo = alunoCodigo;
            TurmaCodigo = turmaCodigo;
            TurmaNome = turmaNome;
            AnoLetivo = anoLetivo;
            SecaoValor = secaoValor;
            SecaoNome = secaoNome;
        }

        public string AlunoCodigo { get; set; }
        public string TurmaCodigo { get; set; }
        public string TurmaNome { get; set; }
        public int AnoLetivo { get; set; }
        public string SecaoNome { get; set; }
        public string SecaoValor { get; set; }
    }
}