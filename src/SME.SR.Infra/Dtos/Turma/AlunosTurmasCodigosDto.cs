using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class AlunosTurmasCodigosDto
    {
        public long AlunoCodigo { get; set; }
        public string TurmaCodigo { get; set; }
        public Modalidade Modalidade { get; set; }
        public int Ano { get; set; }
        public int EtapaEJA { get; set; }
        public string Ciclo { get; set; }
        public string ParecerConclusivo { get; set; }
    }
}
