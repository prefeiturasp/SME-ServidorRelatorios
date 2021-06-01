namespace SME.SR.Infra
{
   public class RelatorioBoletimEscolarDetalhadoDto
    {
        public RelatorioBoletimEscolarDetalhadoDto(BoletimEscolarDetalhadoDto boletimEscolarDetalhado)
        {
            this.BoletimEscolarDetalhado = boletimEscolarDetalhado;
        }

        public BoletimEscolarDetalhadoDto BoletimEscolarDetalhado { get; set; }
    }
}
