using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class SecaoQuestoesEncaminhamentoAeeDto
    {
        public SecaoQuestoesEncaminhamentoAeeDto()
        {
            Questoes = new List<QuestaoEncaminhamentoAeeDto>();
        }
        public string NomeComponenteSecao { get; set; }
        public List<QuestaoEncaminhamentoAeeDto> Questoes { get; set; }

    }
}