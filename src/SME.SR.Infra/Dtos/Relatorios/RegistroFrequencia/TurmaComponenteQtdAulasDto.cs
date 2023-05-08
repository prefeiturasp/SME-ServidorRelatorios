using System;

namespace SME.SR.Infra
{
    public class TurmaComponenteQtdAulasDto
    {
        public string ComponenteCurricularCodigo { get; set; }
        public string TurmaCodigo { get; set; }
        public int AulasQuantidade { get; set; }
        public int Bimestre { get; set; }
        public string Professor { get; set; }
    }
}
