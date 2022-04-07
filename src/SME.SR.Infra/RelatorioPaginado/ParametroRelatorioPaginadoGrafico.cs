using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.RelatorioPaginado
{
    public class ParametroRelatorioPaginadoGrafico
    {
        public List<GraficoBarrasVerticalDto> Graficos {  get; set; }

        public int TotalDeGraficosPorLinha {  get; set; }

        public int TotalDeLinhas { get; set; }  
    }
}
