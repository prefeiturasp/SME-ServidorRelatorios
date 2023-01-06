using SME.SR.Infra.Dtos.Relatorios.PlanoAEE;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class QuestaoPlanoAeeDto
    {
        public QuestaoPlanoAeeDto()
        {
            FrequenciaAluno = new List<FrequenciaAlunoPlanoAeeDto>();
        }

        public string Questao { get; set; }
        public long QuestaoId { get; set; }
        public string Resposta { get; set; }
        public long? RespostaId { get; set; }
        public string Justificativa { get; set; }
        public TipoQuestao TipoQuestao { get; set; }
        public int Ordem { get; set; }
        public IEnumerable<FrequenciaAlunoPlanoAeeDto> FrequenciaAluno { get; set; }        
        public IEnumerable<DadosSrmPlanoAeeDto> InformacoesSrm { get; set; }        
    }
}