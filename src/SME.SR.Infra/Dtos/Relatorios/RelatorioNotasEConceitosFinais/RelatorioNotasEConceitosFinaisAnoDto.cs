using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioNotasEConceitosFinaisAnoDto
    {
        public RelatorioNotasEConceitosFinaisAnoDto()
        {
            Bimestres = new List<RelatorioNotasEConceitosFinaisBimestreDto>();
        }
        public string Nome { get; set; }

        public List<RelatorioNotasEConceitosFinaisBimestreDto> Bimestres { get; set; }
    }
}
