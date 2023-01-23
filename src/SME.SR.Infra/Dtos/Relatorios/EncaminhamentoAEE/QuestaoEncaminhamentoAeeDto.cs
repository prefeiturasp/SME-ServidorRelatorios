using SME.SR.Infra.Dtos.Relatorios.PlanoAEE;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class QuestaoEncaminhamentoAeeDto
    {
        public QuestaoEncaminhamentoAeeDto()
        {
            this.Respostas = new List<RespostaEncaminhamentoAeeDto>();
        }

        public string Questao { get; set; }
        public long QuestaoId { get; set; }
        public TipoQuestao TipoQuestao { get; set; }
        public int Ordem { get; set; }
        public string Justificativa { get; set; }
        public List<RespostaEncaminhamentoAeeDto> Respostas { get; set; }
    }
}