using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFaltasFrequenciaDto
    {
        public RelatorioFaltasFrequenciaDto()
        {
            Dres = new List<RelatorioFaltaFrequenciaDreDto>();
        }
        public bool ExibeFaltas { get; set; }
        public bool ExibeFrequencia { get; set; }
        public List<RelatorioFaltaFrequenciaDreDto> Dres { get; set; }
    }
}
