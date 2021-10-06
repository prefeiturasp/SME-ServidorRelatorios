using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SME.SR.Infra.Dtos.Relatorios.NotasEConceitosFinais
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
