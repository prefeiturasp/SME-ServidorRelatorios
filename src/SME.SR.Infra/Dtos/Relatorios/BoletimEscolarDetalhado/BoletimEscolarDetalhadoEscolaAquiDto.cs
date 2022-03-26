namespace SME.SR.Infra
{
   public class BoletimEscolarDetalhadoEscolaAquiDto
    {
        public BoletimEscolarDetalhadoEscolaAquiDto(BoletimEscolarDetalhadoDto boletimEscolarDetalhado)
        {
            this.BoletimEscolarDetalhado = boletimEscolarDetalhado;
        }

        public BoletimEscolarDetalhadoDto BoletimEscolarDetalhado { get; set; }
    }
}
