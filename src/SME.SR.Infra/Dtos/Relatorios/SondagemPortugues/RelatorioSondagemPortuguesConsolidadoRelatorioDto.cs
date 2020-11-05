using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemPortuguesConsolidadoRelatorioDto
    {
        public RelatorioSondagemPortuguesConsolidadoCabecalhoDto Cabecalho { get; set; }
        public List<RelatorioSondagemPortuguesConsolidadoRespostasDto> Respostas { get; set; }
        public RelatorioSondagemPortuguesConsolidadoRelatorioDto()
        {
            Respostas = new List<RelatorioSondagemPortuguesConsolidadoRespostasDto>();
        }
    }
}
