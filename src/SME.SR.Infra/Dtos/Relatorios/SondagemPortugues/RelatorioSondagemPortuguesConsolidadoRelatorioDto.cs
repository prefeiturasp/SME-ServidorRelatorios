using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioSondagemPortuguesConsolidadoRelatorioDto
    {
        public RelatorioSondagemPortuguesConsolidadoCabecalhoDto Cabecalho { get; set; }
        public List<RelatorioSondagemPortuguesConsolidadoRespostaDto> Respostas { get; set; }
        public RelatorioSondagemPortuguesConsolidadoRelatorioDto()
        {
            this.Respostas = new List<RelatorioSondagemPortuguesConsolidadoRespostaDto>();
        }
    }
}
