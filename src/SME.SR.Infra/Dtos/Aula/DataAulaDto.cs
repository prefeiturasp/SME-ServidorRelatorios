using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class DataAulaDto
    {
        public long Id { get; set; }
        public DateTime Data { get; set; }
        public TurmaNomeDto Turma { get; set; }
        public PeriodoEscolarDto PeriodoEscolar { get; set; }
    }
}
