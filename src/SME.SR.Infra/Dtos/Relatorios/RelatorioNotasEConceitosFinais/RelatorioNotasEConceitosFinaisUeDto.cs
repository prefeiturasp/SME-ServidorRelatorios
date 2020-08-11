using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioNotasEConceitosFinaisUeDto
    {
        public RelatorioNotasEConceitosFinaisUeDto()
        {
            Anos = new List<RelatorioNotasEConceitosFinaisAnoDto>();
        }
        public string Codigo { get; set; }
        public string Nome { get; set; }

        public List<RelatorioNotasEConceitosFinaisAnoDto> Anos { get; set; }
    }
}
