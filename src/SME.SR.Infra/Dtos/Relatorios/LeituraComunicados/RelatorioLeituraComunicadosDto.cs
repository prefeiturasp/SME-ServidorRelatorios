using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioLeituraComunicadosDto
    {
        public RelatorioLeituraComunicadosDto()
        {
            Filtro = new FiltroLeituraComunicadosDto();
            LeituraComunicadoDto = new List<LeituraComunicadoDto>();
        }        

        public FiltroLeituraComunicadosDto Filtro { get; set; }

        public IEnumerable<LeituraComunicadoDto> LeituraComunicadoDto { get; set; }

    }
}
