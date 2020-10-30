using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class ResumoPAPTotalEstudantesDto
    {
        public IEnumerable<ResumoPAPTotalAnoDto> Anos { get; set; }
        public double PorcentagemTotal { get; set; }
        public int QuantidadeTotal { get; set; }
    }
}
