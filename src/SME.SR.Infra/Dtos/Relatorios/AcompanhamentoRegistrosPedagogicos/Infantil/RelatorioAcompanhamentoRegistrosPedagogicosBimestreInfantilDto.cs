using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilDto
    {
        public RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilDto(string bimestre)
        {
            Bimestre = bimestre;
            TurmasInfantil = new List<RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilDto>();
        }
        public string Bimestre { get; set; }
        public List<RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilDto> TurmasInfantil { get; set; }
    }
}
