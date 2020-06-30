using System;

namespace SME.SR.Data
{
    public class PeriodoEscolar
    {
        public long Id { get; set; }
        public int Bimestre { get; set; }
        public DateTime PeriodoFim { get; set; }
        public DateTime PeriodoInicio { get; set; }
    }
}
