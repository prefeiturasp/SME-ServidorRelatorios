namespace SME.SR.Infra
{
    public class PendenciaParaFechamentoConsolidadoDto
    {
        public long PendenciaId { get; set; }
        public string TurmaCodigo { get; set; }
        public int Bimestre { get; set; }
        public long ComponenteCurricularId { get; set; }
        public string Descricao { get; set; }
        public TipoPendencia TipoPendencia { get; set; }
    }
}
