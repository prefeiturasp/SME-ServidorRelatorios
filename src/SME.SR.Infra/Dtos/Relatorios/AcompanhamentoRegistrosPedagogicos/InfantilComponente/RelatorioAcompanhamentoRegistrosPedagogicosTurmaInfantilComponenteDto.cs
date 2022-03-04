namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilComponenteDto
    {
        public string Nome { get; set; }
        public int Aulas { get; set; }
        public int FrequenciasPendentes { get; set; }
        public string DataUltimoRegistroFrequencia { get; set; }
    }
}
