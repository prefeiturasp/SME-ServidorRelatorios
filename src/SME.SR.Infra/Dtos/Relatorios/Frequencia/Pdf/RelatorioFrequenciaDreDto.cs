using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFrequenciaDreDto
    {
        public RelatorioFrequenciaDreDto()
        {
            Ues = new List<RelatorioFrequenciaUeDto>();
        }
        public string CodigoDre { get; set; }
        public string NomeDre { get; set; }

        public List<RelatorioFrequenciaUeDto> Ues { get; set; }
    }
}
