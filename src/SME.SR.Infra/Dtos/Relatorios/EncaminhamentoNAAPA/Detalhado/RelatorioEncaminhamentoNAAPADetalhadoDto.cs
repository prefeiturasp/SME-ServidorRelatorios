using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioEncaminhamentoNAAPADetalhadoDto
    {
        public RelatorioEncaminhamentoNAAPADetalhadoDto()
        {
            Cabecalho = new CabecalhoEncaminhamentoNAAPADetalhadoDto();
            Detalhes = new DetalhesEncaminhamentoNAAPADetalhadoDto();
            AnexosPdf = new List<ArquivoDto>();
        }

        public CabecalhoEncaminhamentoNAAPADetalhadoDto Cabecalho { get; set; }
        public DetalhesEncaminhamentoNAAPADetalhadoDto Detalhes { get; set; }
        public IEnumerable<ArquivoDto> AnexosPdf { get; set; }
    }
}
