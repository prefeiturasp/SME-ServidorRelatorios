using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class VisaoSemanalControleGradeSinteticoDto
    {
        public string Data { get; set; }
        public int DiasLetivo { get; set; }
        public int QuantidadeGrade { get; set; }
        public int AulasCriadas { get; set; }
        public string Diferenca { get; set; }
    }
}
