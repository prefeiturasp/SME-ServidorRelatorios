using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioPendenciasDreDto
    {
        public RelatorioPendenciasDreDto()
        {
            
        }
        public string Codigo { get; set; }
        public string Nome { get; set; }

        public RelatorioPendenciasUeDto Ue { get; set; }
    }
}
