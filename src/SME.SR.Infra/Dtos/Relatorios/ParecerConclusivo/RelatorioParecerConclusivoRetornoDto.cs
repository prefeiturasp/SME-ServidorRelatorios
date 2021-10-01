namespace SME.SR.Infra
{
    public class RelatorioParecerConclusivoRetornoDto
    {
        public long TurmaId { get; set; }
        public int AlunoCodigo { get; set; }
        public string ParecerConclusivo { get; set; }
        public string DreNome { get; set; }
        public string DreCodigo { get; set; }
        public string UeNome { get; set; }
        public string UeCodigo { get; set; }
        public string Ano { get; set; }
        public string AnoLetivo { get; set; }
        public string Ciclo { get; set; }
        public long CicloId { get; set; }
        public string TurmaNome { get; set; }
    }
}
