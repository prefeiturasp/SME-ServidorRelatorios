using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioControlePlanejamentoDiarioComponenteDto
    {
        public RelatorioControlePlanejamentoDiarioComponenteDto()
        {
            Filtro = new FiltroControlePlanejamentoDiarioDto();
        }
        public FiltroControlePlanejamentoDiarioDto Filtro { get; set; }
        public IEnumerable<BimestrePlanejamentoDiarioDto> Bimestres { get; set; }
    }
}
