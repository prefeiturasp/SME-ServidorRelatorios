namespace SME.SR.Infra
{
    public class TurmaResumoDto
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string AnoLetivo { get; set; }
        public string Codigo { get; set; }
        public Modalidade Modalidade { get; set; }

        public UeDto Ue { get; set; }
    }
}
