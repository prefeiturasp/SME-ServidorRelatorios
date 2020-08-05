using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioParecerConclusivoDreDto
    {
        public RelatorioParecerConclusivoDreDto()
        {
            Ues = new List<RelatorioParecerConclusivoUeDto>();
        }
        public string Codigo { get; set; }
        public string Nome { get; set; }

        public List<RelatorioParecerConclusivoUeDto> Ues { get; set; }
    }
}