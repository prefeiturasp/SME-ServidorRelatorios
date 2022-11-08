using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class QuestaoDto
    {
        public long Id { get; set; }
        public int Ordem { get; set; }
        public string Nome { get; set; }
        public TipoQuestao Tipo { get; set; }
        public OpcaoRespostaDto[] OpcaoResposta { get; set; }
        public IEnumerable<RespostaQuestaoDto> Respostas { get; set; }        
    }
}