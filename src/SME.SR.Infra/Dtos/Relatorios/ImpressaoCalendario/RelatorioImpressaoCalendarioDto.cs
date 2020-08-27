using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioImpressaoCalendarioDto
    {
        public string DreNome { get; set; }
        public string UeNome { get; set; }
        public string TipoCalendarioNome { get; set; }
        public IEnumerable<RelatorioImpressaoCalendarioMesDto> Meses { get; set; }

    }
}
