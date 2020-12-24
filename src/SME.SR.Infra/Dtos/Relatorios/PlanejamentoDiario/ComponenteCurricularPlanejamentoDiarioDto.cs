using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class ComponenteCurricularPlanejamentoDiarioDto
    {
        public ComponenteCurricularPlanejamentoDiarioDto()
        {
            PlanejamentoDiario = new List<PlanejamentoDiarioDto>();
            PlanejamentoDiarioInfantil = new List<PlanejamentoDiarioInfantilDto>();
        }

        public string Nome { get; set; }
        public IEnumerable<PlanejamentoDiarioDto> PlanejamentoDiario { get; set; }
        public IEnumerable<PlanejamentoDiarioInfantilDto> PlanejamentoDiarioInfantil { get; set; }
    }
}
