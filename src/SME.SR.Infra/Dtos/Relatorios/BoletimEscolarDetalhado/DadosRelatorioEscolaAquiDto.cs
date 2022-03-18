namespace SME.SR.Infra
{
   public class DadosRelatorioEscolaAquiDto
    {
        public DadosRelatorioEscolaAquiDto(BoletimEscolarDetalhadoDto boletimEscolarDetalhado)
        {
            this.BoletimEscolarDetalhado = boletimEscolarDetalhado;
        }

        public BoletimEscolarDetalhadoDto BoletimEscolarDetalhado { get; set; }
    }
}
