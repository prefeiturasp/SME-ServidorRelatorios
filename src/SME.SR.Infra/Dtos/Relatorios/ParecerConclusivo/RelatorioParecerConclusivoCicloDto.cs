using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioParecerConclusivoCicloDto
    {
        public RelatorioParecerConclusivoCicloDto()
        {
            Anos = new List<RelatorioParecerConclusivoAnoDto>();
        }
        public string Codigo { get; set; }
        public string Nome { get; set; }

        public List<RelatorioParecerConclusivoAnoDto> Anos { get; set; }

    }
}