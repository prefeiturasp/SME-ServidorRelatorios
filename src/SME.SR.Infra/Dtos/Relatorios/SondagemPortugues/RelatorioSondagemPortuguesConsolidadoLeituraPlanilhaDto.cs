using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaDto
    {
        public string Ordem;

        public List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaPerguntaDto> Perguntas;
        public RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaDto()
        {
            Perguntas = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaPerguntaDto>();
        }
    }
}
