using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoRegistrosPedagogicosDto
    {
        public RelatorioAcompanhamentoRegistrosPedagogicosDto()
        {
            Bimestre = new List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto>();
        }
        public RelatorioAcompanhamentoRegistrosPedagogicosCabecalhoDto Cabecalho { get; set; }
        public List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto> Bimestre { get; set; }
    }
}
