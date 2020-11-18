using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class ResumoPAPGraficoAnoDto
    {
        public ResumoPAPGraficoAnoDto(decimal larguraTotal, string titulo)
        {
            EixosX = new List<GraficoBarrasPAPVerticalEixoXDto>();
            EixoYConfiguracao = new GraficoBarrasPAPVerticalEixoYDto(10, "", 10, 10);
            Titulo = titulo;
        }

        public List<GraficoBarrasPAPVerticalEixoXDto> EixosX { get; set; }

        public GraficoBarrasPAPVerticalEixoYDto EixoYConfiguracao { get; set; }

        public string Titulo { get; set; }

    }
}
