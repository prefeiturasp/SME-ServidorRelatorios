using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoRegistrosPedagogicosDto
    {
        public RelatorioAcompanhamentoRegistrosPedagogicosDto()
        {

        }
        public RelatorioAcompanhamentoRegistrosPedagogicosDto(List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto> bimestres, RelatorioAcompanhamentoRegistrosPedagogicosCabecalhoDto cabecalho)
        {
            Bimestre = bimestres;
            Cabecalho = cabecalho;
        }
        public RelatorioAcompanhamentoRegistrosPedagogicosCabecalhoDto Cabecalho { get; set; }
        public List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto> Bimestre { get; set; }
    }
}
