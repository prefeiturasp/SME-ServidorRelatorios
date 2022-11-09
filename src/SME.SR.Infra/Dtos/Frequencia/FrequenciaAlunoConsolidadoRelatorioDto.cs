namespace SME.SR.Infra
{
    public class FrequenciaAlunoConsolidadoRelatorioDto
    {
        public string TurmaCodigo { get; set; }
        public string ComponenteCurricularId { get; set; }
        public int Bimestre { get; set; }
        public string AlunoCodigo { get; set; }
        public int TotalAulas { get; set; }
        public int TotalPresencas { get; set; }
        public int TotalRemotos { get; set; }
        public int TotalAusencias { get; set; }
        public int TotalCompensacoes { get; set; }
    }
}
