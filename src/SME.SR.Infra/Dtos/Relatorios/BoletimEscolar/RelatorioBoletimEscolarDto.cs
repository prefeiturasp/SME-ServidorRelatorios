using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioBoletimEscolarDto
    {
        public RelatorioBoletimEscolarDto(BoletimEscolarDto relatorioBoletimEscolar)
        {
            this.RelatorioBoletimEscolar = relatorioBoletimEscolar;
        }

        public BoletimEscolarDto RelatorioBoletimEscolar { get; set; }
    }
}
