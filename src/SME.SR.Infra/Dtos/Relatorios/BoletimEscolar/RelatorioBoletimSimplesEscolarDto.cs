using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioBoletimSimplesEscolarDto
    {
        public RelatorioBoletimSimplesEscolarDto(List<BoletimSimplesEscolarDto> relatorioBoletimEscolar)
        {
            this.RelatorioBoletimEscolar = relatorioBoletimEscolar;
        }

        public List<BoletimSimplesEscolarDto> RelatorioBoletimEscolar { get; set; }
    }
}
