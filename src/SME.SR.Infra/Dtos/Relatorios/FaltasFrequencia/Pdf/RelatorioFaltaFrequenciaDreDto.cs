using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFaltaFrequenciaDreDto
    {
        public RelatorioFaltaFrequenciaDreDto()
        {
            Ues = new List<RelatorioFaltaFrequenciaUeDto>();
        }
        public string CodigoDre { get; set; }
        public string NomeDre { get; set; }

        public List<RelatorioFaltaFrequenciaUeDto> Ues { get; set; }
    }
}
