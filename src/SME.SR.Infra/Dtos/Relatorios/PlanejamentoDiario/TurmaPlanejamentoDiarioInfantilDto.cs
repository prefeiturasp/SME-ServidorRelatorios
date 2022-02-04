using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class TurmaPlanejamentoDiarioInfantilDto
    {
        public TurmaPlanejamentoDiarioInfantilDto()
        {
            Bimestres = new List<BimestrePlanejamentoDiarioInfantilDto>();
        }

        public string Nome { get; set; }
        public IEnumerable<BimestrePlanejamentoDiarioInfantilDto> Bimestres { get; set; }
    }
}
