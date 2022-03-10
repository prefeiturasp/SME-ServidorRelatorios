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

        public RelatorioAcompanhamentoRegistrosPedagogicosInfantilDto(List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilDto> bimestre,
            RelatorioAcompanhamentoRegistrosPedagogicosCabecalhoDto cabecalho)
        {
            Cabecalho = cabecalho;
            Bimestre = bimestre;
        }

        public RelatorioAcompanhamentoRegistrosPedagogicosCabecalhoDto Cabecalho { get; set; }
        public List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilDto> Bimestre { get; set; }

    }
}
