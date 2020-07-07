using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFaltaFrequenciaAnoDto
    {
        public RelatorioFaltaFrequenciaAnoDto()
        {
            Bimestres = new List<RelatorioFaltaFrequenciaBimestreDto>();
        }
        public string Nome { get; set; }
        public List<RelatorioFaltaFrequenciaBimestreDto> Bimestres { get; set; }
    }
}
