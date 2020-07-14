using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioPendenciasFechamentoDto
    {
        public RelatorioCabecalhoPendenciaFechamentoDto Cabecalho { get; set; }

        public List<PendenciaDetalhamentoDto> PendenciaDetalhamento { get; set; }

        public RelatorioPendenciasFechamentoDto()
        {
            PendenciaDetalhamento = new List<PendenciaDetalhamentoDto>();
        }
    }
}
