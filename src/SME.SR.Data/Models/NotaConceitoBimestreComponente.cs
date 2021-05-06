﻿using System.Globalization;

namespace SME.SR.Data
{
    public class NotaConceitoBimestreComponente
    {
        public string AlunoCodigo { get; set; }
        public int? Bimestre { get; set; }
        public long ComponenteCurricularCodigo { get; set; }
        public long? ConceitoId { get; set; }
        public string Conceito { get; set; }
        public double Nota { get; set; }
        public string Sintese { get; set; }
        public string NotaConceito { get => ConceitoId.HasValue ? Conceito : Nota.ToString("0.0", CultureInfo.InvariantCulture); }
        public long ConselhoClasseAlunoId { get; set; }
    }
}
