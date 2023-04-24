namespace SME.SR.Infra
{
    public class RelatorioEncaminhamentoNAAPADetalhadoDto
    {
        public RelatorioEncaminhamentoNAAPADetalhadoDto()
        {
            Cabecalho = new CabecalhoEncaminhamentoNAAPADetalhadoDto();
            Detalhes = new DetalhesEncaminhamentoNAAPADetalhadoDto();
        }

        public CabecalhoEncaminhamentoNAAPADetalhadoDto Cabecalho { get; set; }
        public DetalhesEncaminhamentoNAAPADetalhadoDto Detalhes { get; set; }
    }
}
