using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemPortuguesConsolidadoLeituraRelatorioDto
    {
        public RelatorioSondagemPortuguesConsolidadoLeituraCabecalhoDto Cabecalho { get; set; }
        public List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaDto> Planilhas { get; set; }
        public RelatorioSondagemPortuguesConsolidadoLeituraRelatorioDto()
        {
            Planilhas = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaDto>();
        }
        public List<GraficoBarrasVerticalDto> GraficosBarras { get; set; }
    }
}
