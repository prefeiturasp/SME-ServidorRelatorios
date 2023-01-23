using SME.SR.Infra.Dtos.Relatorios.PlanoAEE;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class QuestaoEncaminhamentoAeeDto
    {
        public QuestaoEncaminhamentoAeeDto()
        {
            AtendimentoClinico = new List<AtendimentoClinicoAlunoDto>();
            InformacaoEscolar = new List<InformacaoEscolarAlunoDto>();
        }

        public string Questao { get; set; }
        public long QuestaoId { get; set; }
        public TipoQuestao TipoQuestao { get; set; }
        public int Ordem { get; set; }
        public string Resposta { get; set; }
        public List<AtendimentoClinicoAlunoDto> AtendimentoClinico { get; set; }
        public List<InformacaoEscolarAlunoDto> InformacaoEscolar { get; set; }
    }
}