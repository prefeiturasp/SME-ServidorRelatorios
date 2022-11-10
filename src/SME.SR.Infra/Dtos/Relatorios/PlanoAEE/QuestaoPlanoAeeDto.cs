using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class QuestaoPlanoAeeDto
    {
        public string Questao { get; set; }
        public long QuestaoId { get; set; }
        public string Resposta { get; set; }
        public long? RespostaId { get; set; }
        public string Justificativa { get; set; }
        public TipoQuestao TipoQuestao { get; set; }
        public int Ordem { get; set; }
        public IEnumerable<FrequenciaAlunoPlanoAeeDto> FrequenciaAluno { get; set; }        
    }
}