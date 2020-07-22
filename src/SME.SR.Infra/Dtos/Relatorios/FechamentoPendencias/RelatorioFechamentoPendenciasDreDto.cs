using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFechamentoPendenciasDreDto
    {
        public RelatorioFechamentoPendenciasDreDto()
        {
            
        }
        public string Codigo { get; set; }
        public string Nome { get; set; }

        public RelatorioFechamentoPendenciasUeDto Ue { get; set; }
    }
}
