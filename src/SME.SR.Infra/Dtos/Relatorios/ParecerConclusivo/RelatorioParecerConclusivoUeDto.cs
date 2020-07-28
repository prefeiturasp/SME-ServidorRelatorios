using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioParecerConclusivoUeDto
    {
        public RelatorioParecerConclusivoUeDto()
        {
            Ciclos = new List<RelatorioParecerConclusivoCicloDto>();
        }
        public string Codigo { get; set; }
        public string Nome { get; set; }

        public List<RelatorioParecerConclusivoCicloDto> Ciclos { get; set; }
    }
}