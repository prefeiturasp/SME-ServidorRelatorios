namespace SME.SR.Infra.Dtos
{
    public class PaginaParaRelatorioPaginacaoSoloDto
    {
        public PaginaParaRelatorioPaginacaoSoloDto(string html, int pagina, int total)
        {
            Html = html;
            Pagina = pagina;
            Total = total;
        }

        public string Html { get; set; }
        public int Pagina { get; set; }
        public int Total { get; set; }
    }
}