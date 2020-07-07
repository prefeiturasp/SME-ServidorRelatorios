using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFaltasFrequenciaDto
    {
        public RelatorioFaltasFrequenciaDto()
        {
            Dres = new List<RelatorioFaltaFrequenciaDreDto>();
        }

        public List<RelatorioFaltaFrequenciaDreDto> Dres { get; set; }
    }
}
