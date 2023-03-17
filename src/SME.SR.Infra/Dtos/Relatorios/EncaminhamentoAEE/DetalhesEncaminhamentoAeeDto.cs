using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class DetalhesEncaminhamentoAeeDto
    {
        public SecaoQuestoesEncaminhamentoAeeDto InformacoesEscolares { get; set; }
        public SecaoQuestoesEncaminhamentoAeeDto DescricaoEncaminhamento { get; set; }
        public SecaoQuestoesEncaminhamentoAeeDto ParecerCoordenacao { get; set; }
        public SecaoQuestoesEncaminhamentoAeeDto ParecerAee { get; set; }
    }
}