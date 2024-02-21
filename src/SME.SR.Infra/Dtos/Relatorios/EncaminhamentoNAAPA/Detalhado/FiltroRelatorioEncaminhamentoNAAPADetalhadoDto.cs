namespace SME.SR.Infra.EncaminhamentoNaapa
{
    public class FiltroRelatorioEncaminhamentoNAAPADetalhadoDto
    {
        public long[] EncaminhamentoNaapaIds { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioRf { get; set; }
        public ImprimirAnexosNAAPA ImprimirAnexos { get; set; }
    }
}