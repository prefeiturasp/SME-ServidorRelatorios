using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class SecaoQuestoesEncaminhamentoNAAPADetalhadoDto
    {
        public SecaoQuestoesEncaminhamentoNAAPADetalhadoDto(string nomeComponenteSecao, string nomeSecao)
        {
            NomeComponenteSecao = nomeComponenteSecao;
            NomeSecao = nomeSecao;
            Questoes = new List<QuestaoEncaminhamentoNAAPADetalhadoDto>();
        }

        public string NomeComponenteSecao { get; set; }
        public string NomeSecao { get; set; }
        public List<QuestaoEncaminhamentoNAAPADetalhadoDto> Questoes { get; set; }
    }
}
