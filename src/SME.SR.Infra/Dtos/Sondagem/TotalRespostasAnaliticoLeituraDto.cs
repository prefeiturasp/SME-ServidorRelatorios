namespace SME.SR.Infra.Dtos.Sondagem
{
    public class TotalRespostasAnaliticoLeituraDto
    {
        public int Nivel1 { get; set; }
        public int Nivel2 { get; set; }
        public int Nivel3 { get; set; }
        public int Nivel4 { get; set; }
        public int SemPreenchimento { get; set; }
        public string TurmaCodigo { get; set; }
        public string AnoTurma { get; set; }
        public string CodigoUe { get; set; }
        public string CodigoDre { get; set; }
    }
}