using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class PeriodoEscolarDto
    {
        public long Id { get; set; }
        public int Bimestre { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}
