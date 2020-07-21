using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFechamentoPendenciasDreDto
    {
        public RelatorioFechamentoPendenciasDreDto()
        {
            Ues = new List<RelatorioFechamentoPendenciasUeDto>();
        }
        public string Codigo { get; set; }
        public string Nome { get; set; }

        public List<RelatorioFechamentoPendenciasUeDto> Ues { get; set; }
    }
}
