namespace SME.SR.Infra
{
    public class RespostaSondagemAnaliticoLeituraDto : RelatorioSondagemAnaliticoDto
    {
        public int Nivel1 { get; set; }
        public int Nivel2 { get; set; }
        public int Nivel3 { get; set; }
        public int Nivel4 { get; set; }
        public int SemPreenchimento { get; set; }
    }
}
