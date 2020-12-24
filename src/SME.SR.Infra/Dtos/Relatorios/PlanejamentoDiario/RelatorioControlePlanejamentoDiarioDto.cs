using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioControlePlanejamentoDiarioDto
    {
        public RelatorioControlePlanejamentoDiarioDto()
        {
            Filtro = new FiltroControlePlanejamentoDiarioDto();
            Turmas = new List<TurmaPlanejamentoDiarioDto>();            
        }

        public FiltroControlePlanejamentoDiarioDto Filtro { get; set; }

        public IEnumerable<TurmaPlanejamentoDiarioDto> Turmas { get; set; }

    }
}
