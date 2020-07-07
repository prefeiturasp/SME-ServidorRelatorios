using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFaltaFrequenciaDreDto
    {
        public RelatorioFaltaFrequenciaDreDto()
        {
            Ues = new List<RelatorioFaltaFrequenciaUeDto>();
        }
        public string Codigo { get; set; }
        public string Nome { get; set; }

        public List<RelatorioFaltaFrequenciaUeDto> Ues { get; set; }
    }
}
