using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFaltaFrequenciaBimestreDto
    {
        public RelatorioFaltaFrequenciaBimestreDto()
        {
            Componentes = new List<RelatorioFaltaFrequenciaComponenteDto>();
        }
        public string NomeBimestre { get; set; }
        public List<RelatorioFaltaFrequenciaComponenteDto> Componentes { get; set; }
    }
}
