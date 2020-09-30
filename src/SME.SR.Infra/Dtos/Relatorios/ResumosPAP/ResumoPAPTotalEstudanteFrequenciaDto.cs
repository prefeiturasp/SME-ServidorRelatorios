using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class ResumoPAPTotalEstudanteFrequenciaDto
    {
        public string FrequenciaDescricao { get; set; }
        public List<ResumoPAPFrequenciaDto> Linhas { get; set; }
        public double PorcentagemTotalFrequencia { get; set; }
        public int QuantidadeTotalFrequencia { get; set; }
    }
}
