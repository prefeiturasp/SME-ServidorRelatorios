namespace SME.SR.Data
{
    public class NotaConceitoBimestreComponente
    {
        public int? Bimestre { get; set; }
        public long ComponenteCurricularCodigo { get; set; }
        public long? ConceitoId { get; set; }
        public double Nota { get; set; }
        public double NotaConceito { get => ConceitoId.HasValue ? ConceitoId.Value : Nota; }
    }
}
