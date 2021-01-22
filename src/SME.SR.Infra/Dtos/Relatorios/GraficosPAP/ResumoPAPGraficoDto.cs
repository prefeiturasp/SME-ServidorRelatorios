using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class ResumoPAPGraficoDto
    {
        public ResumoPAPGraficoDto()
        {
            Graficos = new List<ResumoPAPGraficoAnoDto>();
        }

        public string Titulo { get; set; }

        public List<ResumoPAPGraficoAnoDto> Graficos { get; set; }
    }
}
