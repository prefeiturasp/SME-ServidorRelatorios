namespace SME.SR.Infra
{
   public class RelatorioBoletimEscolarDetalhadoDto
    {
        public RelatorioBoletimEscolarDetalhadoDto(BoletimEscolarDetalhadoDto relatorioBoletimEscolarDetalhado)
        {
            this.BoletimEscolarDetalhado = boletimEscolarDetalhado;
        }

        public BoletimEscolarDetalhadoDto RelatorioBoletimEscolarDetalhado { get; set; }
    }
}
