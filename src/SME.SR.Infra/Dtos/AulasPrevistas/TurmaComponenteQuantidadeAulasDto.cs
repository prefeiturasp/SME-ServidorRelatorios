namespace SME.SR.Infra.Dtos.AulasPrevistas
{
    public class TurmaComponenteQuantidadeAulasDto
    {
        public string ComponenteCurricularCodigo { get; set; }
        public string TurmaCodigo { get; set; }
        public int AulasQuantidade { get; set; }
        public int Bimestre { get; set; }
        public long PeriodoEscolarId { get; set; }
    }
}
