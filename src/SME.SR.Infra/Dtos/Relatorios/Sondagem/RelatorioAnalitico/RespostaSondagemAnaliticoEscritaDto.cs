namespace SME.SR.Infra
{
    public class RespostaSondagemAnaliticoEscritaDto : RelatorioSondagemAnaliticoDto
    {
        public int PreSilabico { get; set; }
        public int SilabicoSemValor { get; set; }
        public int SilabicoComValor { get; set; }
        public int SilabicoAlfabetico { get; set; }
        public int Alfabetico { get; set; }
        public int Nivel1 { get; set; }
        public int Nivel2 { get; set; }
        public int Nivel3 { get; set; }
        public int Nivel4 { get; set; }
        public int SemPreenchimento { get; set; }
    }
}
