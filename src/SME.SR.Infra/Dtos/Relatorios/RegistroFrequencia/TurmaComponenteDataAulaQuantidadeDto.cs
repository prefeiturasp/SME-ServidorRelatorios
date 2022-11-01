using System;

namespace SME.SR.Infra
{
    public class TurmaComponenteDataAulaQuantidadeDto
    {
        public DateTime DataAula { get; set; }
        public string ComponenteCurricularCodigo { get; set; }
        public string TurmaCodigo { get; set; }
        public int QuantidadeAula { get; set; }
        public int Bimestre { get; set; }
    }
}
