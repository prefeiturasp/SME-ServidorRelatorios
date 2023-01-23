using SME.SR.Infra.Dtos.Relatorios.PlanoAEE;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RespostaEncaminhamentoAeeDto
    {
        public RespostaEncaminhamentoAeeDto()
        {
            AtendimentoClinico = new List<AtendimentoClinicoAlunoDto>();
            InformacaoEscolar = new List<InformacaoEscolarAlunoDto>();
        }
        public string Resposta { get; set; }
        public long? RespostaId { get; set; }
        public string Justificativa { get; set; }

        public List<AtendimentoClinicoAlunoDto> AtendimentoClinico { get; set; }
        public List<InformacaoEscolarAlunoDto> InformacaoEscolar { get; set; }
    }
}