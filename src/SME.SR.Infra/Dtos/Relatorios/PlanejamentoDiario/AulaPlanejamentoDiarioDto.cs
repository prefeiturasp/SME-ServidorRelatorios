using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class AulaPlanejamentoDiarioDto
    {
        public AulaPlanejamentoDiarioDto()
        {
            PlanejamentoDiario = new List<PlanejamentoDiarioDto>();
        }

        public string DataAula { get; set; }
        public IEnumerable<PlanejamentoDiarioDto> PlanejamentoDiario { get; set; }
    }
}
