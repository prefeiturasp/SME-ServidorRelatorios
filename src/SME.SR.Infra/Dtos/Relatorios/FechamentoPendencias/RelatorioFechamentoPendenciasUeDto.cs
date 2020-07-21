using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFechamentoPendenciasUeDto
    {
        public RelatorioFechamentoPendenciasUeDto()
        {
            Anos = new List<RelatorioFechamentoPendenciasAnoDto>();
        }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public List<RelatorioFechamentoPendenciasAnoDto> Anos { get; set; }
    }
}
