using SME.SR.Infra.Dtos.Relatorios.PlanoAEE;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RespostaEncaminhamentoAeeDto
    {
        public RespostaEncaminhamentoAeeDto()
        {}
        public string Resposta { get; set; }
        public long? RespostaId { get; set; }
        public string Justificativa { get; set; }
    }
}