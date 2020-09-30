using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class ResumoPAPFrequenciaDto
    {
        public IEnumerable<ResumoPAPTotalFrequenciaAnoDto> Anos { get; set; }
        public double PorcentagemTotalFrequencia { get; set; }
        public int QuantidadeTotalFrequencia { get; set; }
    }
}
