using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class GraficoBarrasVerticalDto
    {
        public GraficoBarrasVerticalDto()
        {
            EixosX = new List<GraficoBarrasVerticalEixoXDto>();
            EixosY = new List<GraficoBarrasVerticalEixoYDto>();
        }

        public List<GraficoBarrasVerticalEixoXDto> EixosX { get; set; }
        public List<GraficoBarrasVerticalEixoYDto> EixosY { get; set; }
    }
}
