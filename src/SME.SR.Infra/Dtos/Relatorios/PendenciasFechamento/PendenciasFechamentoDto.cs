using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class PendenciasFechamentoDto
    {
        public CabecalhoPendenciaFechamentoDto Cabecalho { get; set; }

        public List<PendenciaDetalhamentoDto> PendenciaDetalhamento { get; set; }
    }
}
