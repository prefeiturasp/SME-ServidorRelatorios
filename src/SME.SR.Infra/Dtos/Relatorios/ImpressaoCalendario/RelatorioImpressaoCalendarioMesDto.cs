using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioImpressaoCalendarioMesDto
    {
        public int MesNumero { get; set; }
        public string MesDescricao { get; set; }

        public IEnumerable<RelatorioImpressaoCalendarioEventoDto> Eventos { get; set; }

    }
}
