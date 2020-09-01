using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioImpressaoCalendarioDto
    {
        public RelatorioImpressaoCalendarioDto()
        {
            Meses = new List<RelatorioImpressaoCalendarioMesDto>();
        }
        public string DreNome { get; set; }
        public string UeNome { get; set; }
        public string TipoCalendarioNome { get; set; }
        public IList<RelatorioImpressaoCalendarioMesDto> Meses { get; set; }

    }
}
