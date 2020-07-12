using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFaltaFrequenciaUeDto
    {
        public RelatorioFaltaFrequenciaUeDto()
        {
            Anos = new List<RelatorioFaltaFrequenciaAnoDto>();
        }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public List<RelatorioFaltaFrequenciaAnoDto> Anos { get; set; }
    }
}
