namespace SME.SR.Infra.Dtos
{
    public class FrequenciaAlunoConsolidadoDto
    {
        public string Bimestre { get; set; }
        public string TotalAula { get; set; }
        public string TotalPresencas { get; set; }
        public string TotalRemotos { get; set; }
        public string TotalAusencias { get; set; }
        public string TotalCompensacoes { get; set; }
        public string CodigoAluno { get; set; }
    }
}
