namespace SME.SR.Infra
{
   public class RelatorioBoletimEscolarDetalhadoDto
    {
        public RelatorioBoletimEscolarDetalhadoDto(BoletimEscolarDetalhadoDto relatorioBoletimEscolarDetalhado)
        {
            this.RelatorioBoletimEscolarDetalhado = relatorioBoletimEscolarDetalhado;
        }

        public BoletimEscolarDetalhadoDto RelatorioBoletimEscolarDetalhado { get; set; }
    }
}
