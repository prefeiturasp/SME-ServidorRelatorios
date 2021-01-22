using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemPortuguesConsolidadoRespostasDto
    {
        public List<RelatorioSondagemPortuguesConsolidadoRespostaDto> Respostas { get; set; }

        public RelatorioSondagemPortuguesConsolidadoRespostasDto()
        {
            this.Respostas = new List<RelatorioSondagemPortuguesConsolidadoRespostaDto>();
        }
    }
}
