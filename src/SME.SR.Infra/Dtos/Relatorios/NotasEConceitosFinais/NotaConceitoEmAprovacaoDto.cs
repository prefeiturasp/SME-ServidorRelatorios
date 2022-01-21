using System.Globalization;

namespace SME.SR.Infra
{
    public class NotaConceitoEmAprovacaoDto
    {
        public long WorkflowId { get; set; }
        public long? ConceitoId { get; set; }
        public string Conceito { get; set; }
        public double? Nota { get; set; }
        public string NotaConceito { get => ConceitoId.HasValue ? Conceito : Nota.HasValue ? Nota.Value.ToString("0.0", CultureInfo.InvariantCulture) : ""; }
    }
}
