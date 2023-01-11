using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class CadastroPlanoAeeDto
    {
        public CadastroPlanoAeeDto()
        {
            Questoes = new List<QuestaoPlanoAeeDto>();
        }

        public string Responsavel { get; set; }
        public IEnumerable<QuestaoPlanoAeeDto> Questoes { get; set; }        
    }
}