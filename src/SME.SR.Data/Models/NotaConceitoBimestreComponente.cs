using System.Globalization;

namespace SME.SR.Data
{
    public class NotaConceitoBimestreComponente
    {
        public int? Bimestre { get; set; }
        public long ComponenteCurricularCodigo { get; set; }
        public long? ConceitoId { get; set; }
        public string Conceito { get; set; }
        public double Nota { get; set; }
        public string NotaConceito { get => ConceitoId.HasValue ? Conceito : Nota.ToString("0.0", CultureInfo.InvariantCulture); }
    }
}
