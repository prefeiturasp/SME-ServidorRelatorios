using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class TurmaPlanejamentoDiarioDto
    {
        public TurmaPlanejamentoDiarioDto()
        {
            Bimestres = new List<BimestrePlanejamentoDiarioDto>();
        }

        public string Nome { get; set; }
        public IEnumerable<BimestrePlanejamentoDiarioDto> Bimestres { get; set; }
    }
}
