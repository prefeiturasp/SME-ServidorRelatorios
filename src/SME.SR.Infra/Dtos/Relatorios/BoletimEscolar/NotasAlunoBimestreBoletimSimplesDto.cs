using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class NotasAlunoBimestreBoletimSimplesDto
    {
        public string CodigoTurma { get; set; }
        public string CodigoAluno { get; set; }
        public string CodigoComponenteCurricular { get; set; }
        public string NotaConceito { get; set; }
        public int Bimestre { get; set; }
    }
}
