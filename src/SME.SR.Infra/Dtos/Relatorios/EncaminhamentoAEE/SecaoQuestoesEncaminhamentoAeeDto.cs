using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class SecaoQuestoesEncaminhamentoAeeDto
    {
        public SecaoQuestoesEncaminhamentoAeeDto()
        {
            Questoes = new List<QuestaoEncaminhamentoAeeDto>();
            AtendimentoClinico = new List<AtendimentoClinicoAlunoDto>();
            InformacaoEscolar = new List<InformacaoEscolarAlunoDto>();
        }
        public string NomeComponenteSecao { get; set; }
        public List<QuestaoEncaminhamentoAeeDto> Questoes { get; set; }
        public List<AtendimentoClinicoAlunoDto> AtendimentoClinico { get; set; }
        public List<InformacaoEscolarAlunoDto> InformacaoEscolar { get; set; }

    }
}