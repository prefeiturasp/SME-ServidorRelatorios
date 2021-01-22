using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class AulaDiasNaoLetivosControleGradeDto
    {
        public string Data { get; set; }
        public string Professor { get; set; }
        public string Motivo { get; set; }
        public int QuantidadeAulas { get; set; }
    }
}
