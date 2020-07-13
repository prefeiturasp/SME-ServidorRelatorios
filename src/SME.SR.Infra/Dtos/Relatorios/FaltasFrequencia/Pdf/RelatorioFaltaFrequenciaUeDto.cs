using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFaltaFrequenciaUeDto
    {
        public RelatorioFaltaFrequenciaUeDto()
        {
            Anos = new List<RelatorioFaltaFrequenciaAnoDto>();
        }
        public string CodigoUe { get; set; }
        public string NomeUe { get; set; }
        public List<RelatorioFaltaFrequenciaAnoDto> Anos { get; set; }
    }
}
