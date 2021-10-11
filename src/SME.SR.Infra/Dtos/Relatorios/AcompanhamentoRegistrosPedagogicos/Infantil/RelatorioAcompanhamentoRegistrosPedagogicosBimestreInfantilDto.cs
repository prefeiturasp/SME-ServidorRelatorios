using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilDto
    {
        public string Bimestre { get; set; }
        public List<RelatorioAcompanhamentRegistrosPedagogicosTurmaInfantilDto> TurmasInfantil { get; set; }
    }
}
