using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class GraficoBarrasVerticalDto
    {
        public GraficoBarrasVerticalDto(decimal larguraTotal, string titulo)
        {
            EixosX = new List<GraficoBarrasVerticalEixoXDto>();
            LarguraTotal = larguraTotal;
            EixoYConfiguracao = new GraficoBarrasVerticalEixoYDto(10, "", 10, 10);
            Titulo = titulo;
            IdParaLastro = Guid.NewGuid().ToString().Replace("-", "");
        }

        public List<GraficoBarrasVerticalEixoXDto> EixosX { get; set; }
        public GraficoBarrasVerticalEixoYDto EixoYConfiguracao { get; set; }
        public decimal LarguraTotal { get; set; }

        public string Titulo { get; set; }

        public string IdParaLastro { get; set; }

    }
}
