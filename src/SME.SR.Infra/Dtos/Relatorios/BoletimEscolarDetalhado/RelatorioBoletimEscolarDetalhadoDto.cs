namespace SME.SR.Infra
{
   public class RelatorioBoletimEscolarDetalhadoDto
    {
        public RelatorioBoletimEscolarDetalhadoDto(BoletimEscolarDto relatorioBoletimEscolarDetalhado)
        {
            this.RelatorioBoletimEscolarDetalhado = relatorioBoletimEscolarDetalhado;
        }

        public BoletimEscolarDto RelatorioBoletimEscolarDetalhado { get; set; }
    }
}
