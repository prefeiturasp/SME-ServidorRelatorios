using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilDto
    {
        public int Bimestre { get; set; }
        public List<RelatorioAcompanhamentRegistrosPedagogicosTurmaInfantilDto> TurmasInfantil { get; set; }
    }
}
