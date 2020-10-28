using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaPerguntaDto
    {
        public string Pergunta { get; set; }
        public List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto> Respostas { get; set; }

        public RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaPerguntaDto()
        {
            this.Respostas = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto>();
        }
    }
}
