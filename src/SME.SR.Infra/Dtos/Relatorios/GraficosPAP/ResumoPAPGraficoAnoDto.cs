using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class ResumoPAPGraficoAnoDto
    {
        public ResumoPAPGraficoAnoDto(decimal larguraTotal, string titulo)
        {
            EixosX = new List<GraficoBarrasPAPVerticalEixoXDto>();
            LarguraTotal = larguraTotal;
            EixoYConfiguracao = new GraficoBarrasPAPVerticalEixoYDto(10, "", 10, 10);
            Titulo = titulo;
            IdParaLastro = Guid.NewGuid().ToString().Replace("-", "");
        }

        public List<GraficoBarrasPAPVerticalEixoXDto> EixosX { get; set; }

        public GraficoBarrasPAPVerticalEixoYDto EixoYConfiguracao { get; set; }
        public decimal LarguraTotal { get; set; }

        public string Titulo { get; set; }
        public string IdParaLastro { get; set; }

    }
}
