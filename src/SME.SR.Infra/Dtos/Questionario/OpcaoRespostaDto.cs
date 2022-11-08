using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class OpcaoRespostaDto
    {
        public long Id { get; set; }
        public long QuestaoId { get; set; }
        public int Ordem { get; set; }
        public string Nome { get; set; }
        public List<QuestaoDto> QuestoesComplementares { get; set; }
    }
}