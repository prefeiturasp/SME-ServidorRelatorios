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
        }

        public List<GraficoBarrasVerticalEixoXDto> EixosX { get; set; }
        public GraficoBarrasVerticalEixoYDto EixoYConfiguracao { get; set; }
}
}
