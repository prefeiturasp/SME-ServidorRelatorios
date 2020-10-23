using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class GraficoBarrasVerticalDto
    {
        public GraficoBarrasVerticalDto(decimal larguraTotal)
        {
            EixosX = new List<GraficoBarrasVerticalEixoXDto>();
            LarguraTotal = larguraTotal;
        }

        public List<GraficoBarrasVerticalEixoXDto> EixosX { get; set; }
        public GraficoBarrasVerticalEixoYDto EixoYConfiguracao { get; set; }
        public decimal LarguraTotal { get; set; }


    }
}
