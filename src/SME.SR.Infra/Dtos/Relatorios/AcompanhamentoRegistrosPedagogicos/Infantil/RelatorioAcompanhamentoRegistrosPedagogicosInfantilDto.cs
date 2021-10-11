using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra { 
    public class RelatorioAcompanhamentoRegistrosPedagogicosInfantilDto
    {
        public RelatorioAcompanhamentoRegistrosPedagogicosInfantilDto()
        {
            Bimestre = new List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilDto>();
        }
        public RelatorioAcompanhamentoRegistrosPedagogicosCabecalhoDto Cabecalho { get; set; }
        public List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilDto> Bimestre { get; set; }
    }
}
