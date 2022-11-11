using System.Collections.Generic;

namespace SME.SR.Data
{
    public class OpcaoResposta
    {
        public OpcaoResposta()
        {
            QuestoesComplementares = new List<OpcaoQuestaoComplementar>();
        }

        public long Id { get; set; }
        public Questao Questao { get; set; }
        public long QuestaoId { get; set; }
        public int Ordem { get; set; }
        public string Nome { get; set; }
        public List<OpcaoQuestaoComplementar> QuestoesComplementares { get; }
    }
}