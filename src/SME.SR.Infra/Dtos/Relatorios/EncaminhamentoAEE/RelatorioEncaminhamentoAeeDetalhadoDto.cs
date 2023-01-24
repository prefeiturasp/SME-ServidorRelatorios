using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioEncaminhamentoAeeDetalhadoDto
    {
        public RelatorioEncaminhamentoAeeDetalhadoDto()
        {
            Cabecalho = new CabecalhoEncaminhamentoAeeDetalhadoDto();
            Detalhes = new DetalhesEncaminhamentoAeeDto();
        }

        public CabecalhoEncaminhamentoAeeDetalhadoDto Cabecalho { get; set; }
        public DetalhesEncaminhamentoAeeDto Detalhes { get; set; }

    }
}
