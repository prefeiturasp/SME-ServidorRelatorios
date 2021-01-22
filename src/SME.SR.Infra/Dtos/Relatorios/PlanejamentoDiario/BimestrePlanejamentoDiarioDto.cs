using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class BimestrePlanejamentoDiarioDto
    {
        public BimestrePlanejamentoDiarioDto()
        {
            ComponentesCurriculares = new List<ComponenteCurricularPlanejamentoDiarioDto>();
        }

        public string Nome { get; set; }
        public IEnumerable<ComponenteCurricularPlanejamentoDiarioDto> ComponentesCurriculares { get; set; }
    }
}
