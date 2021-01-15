using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioUsuarioDto
    {
        public FiltroUsuarioDto Filtro { get; set; }

        public DadosRelatorioUsuariosDto DadosRelatorio { get; set; }
    }
}
