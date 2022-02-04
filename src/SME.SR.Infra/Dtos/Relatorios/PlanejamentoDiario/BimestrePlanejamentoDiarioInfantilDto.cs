using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class BimestrePlanejamentoDiarioInfantilDto
    {
        public BimestrePlanejamentoDiarioInfantilDto()
        {
            Planejamento = new List<PlanejamentoDiarioInfantilDto>();
        }

        public string Nome { get; set; }

        public IEnumerable<PlanejamentoDiarioInfantilDto> Planejamento { get; set; }
    }
}
