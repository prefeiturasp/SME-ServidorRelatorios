using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioImpressaoCalendarioMesDto
    {
        public RelatorioImpressaoCalendarioMesDto()
        {
            Eventos = new List<RelatorioImpressaoCalendarioEventoDto>();
        }
        public int MesNumero { get; set; }
        public string MesDescricao { get; set; }

        public IList<RelatorioImpressaoCalendarioEventoDto> Eventos { get; set; }

    }
}
