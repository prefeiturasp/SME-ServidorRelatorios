namespace SME.SR.Infra
{
    public class RelatorioFrequenciaIndividualDadosFrequenciasDto
    {
        public int TotalAulasDadas { get; set; }
        public int TotalPresencas { get; set; }
        public int TotalRemoto { get; set; }
        public int TotalAusencias { get; set; }
        public int TotalCompensacoes { get; set; }
        public string TotalPercentualFrequencia { get; set; }
        public string TotalPercentualFrequenciaFormatado { get; set; }
    }
}
