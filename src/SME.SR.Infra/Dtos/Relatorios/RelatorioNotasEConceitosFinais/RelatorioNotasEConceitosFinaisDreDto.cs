using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioNotasEConceitosFinaisDreDto
    {

        public RelatorioNotasEConceitosFinaisDreDto()
        {
            Ues = new List<RelatorioNotasEConceitosFinaisUeDto>();
        }

        public string Codigo { get; set; }
        public string Nome { get; set; }

        public List<RelatorioNotasEConceitosFinaisUeDto> Ues { get; set; }
    }
}
