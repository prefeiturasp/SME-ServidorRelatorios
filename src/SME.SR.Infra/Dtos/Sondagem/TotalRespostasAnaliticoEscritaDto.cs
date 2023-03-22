namespace SME.SR.Infra.Dtos.Sondagem
{
    public class TotalRespostasAnaliticoEscritaDto
    {
        public string TurmaCodigo { get; set; }
        public int PreSilabico { get; set; }
        public int SilabicoSemValor { get; set; }
        public int SilabicoComValor { get; set; }
        public int SilabicoAlfabetico { get; set; }
        public int Alfabetico { get; set; }
        public int SemPreenchimento { get; set; }
    }
}