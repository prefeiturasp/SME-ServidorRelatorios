using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioUsuarioDto
    {
        public FiltroUsuarioDto Filtro { get; set; }
        public IEnumerable<DreUsuarioDto> Dres { get; set; }
    }
}
