using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class ConselhoClasseAtaFinalCelulaDto
    {
        public long GrupoMatriz { get; set; }
        public long ComponenteCurricular { get; set; }
        public int Coluna { get; set; }
        public string Valor { get; set; }
        public string AlunoCodigo { get; set; }
        public int? Bimestre { get; set; }
        public bool Regencia { get; set;  }
    }
}
