using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioParecerConclusivoDto
    {
        public RelatorioParecerConclusivoDto()
        {
            Dres = new List<RelatorioParecerConclusivoDreDto>();
        }

        public string AnoLetivo { get; set; }
        public string DreNome { get; set; }
        public string UeNome { get; set; }
        public string Ano { get; set; }
        public string Usuario { get; set; }
        public string RF { get; set; }
        public string Data { get; set; }
        public string Ciclo { get; set; }

        public List<RelatorioParecerConclusivoDreDto> Dres { get; set; }
        
    }
}
