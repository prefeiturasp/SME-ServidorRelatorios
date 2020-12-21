using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioControlePlanejamentoDiarioDto
    {
        public RelatorioControlePlanejamentoDiarioDto()
        {
            Filtro = new FiltroControlePlanejamentoDiarioDto();
            PlanejamentoDiarioDto = new List<PlanejamentoDiarioDto>();
            PlanejamentoDiarioInfantilDto = new List<PlanejamentoDiarioInfantilDto>();
        }

        public FiltroControlePlanejamentoDiarioDto Filtro { get; set; }
        public IEnumerable<PlanejamentoDiarioDto> PlanejamentoDiarioDto { get; set; }
        public IEnumerable<PlanejamentoDiarioInfantilDto> PlanejamentoDiarioInfantilDto { get; set; }

    }
}
